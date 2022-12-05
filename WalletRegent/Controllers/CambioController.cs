using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using WalletRegent.DTO;
using System.IO;
using System.Net.Http.Headers;
using WalletRegent.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Net;

namespace WalletRegent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CambioController : ControllerBase
    {
        private IQuotationHttpService _quotationSvc;
        private IExchangeWalletHttpService _exchangeWalletSvc;
        private IWalletHttpService _walletSvc;


        public CambioController(IQuotationHttpService quotationSvc, IExchangeWalletHttpService exchangeWalletSvc, IWalletHttpService walletSvc) 
        {
            _quotationSvc = quotationSvc;
            _exchangeWalletSvc = exchangeWalletSvc;
            _walletSvc = walletSvc;
        }


        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("ObterCotacoes")]
        public async Task<string> ObterCotacoes()
        {   
            DadosCotacaoDTO cotacaoDolar = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_DOLAR);
            
            int milliseconds = 1000;
            Thread.Sleep(milliseconds);

            DadosCotacaoDTO cotacaoEuro = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_EURO);

            var cotacoes = new List<string>();

            cotacoes.Add(cotacaoDolar.GetCotacao(QuotationHttpService.NOME_DOLAR));
            cotacoes.Add(cotacaoEuro.GetCotacao(QuotationHttpService.NOME_EURO));
                       
            return JsonConvert.SerializeObject(cotacoes);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CarteiraCambialDTO>), StatusCodes.Status200OK)]
        [Route("ListarDadosDeCarteirasDeCambio")]        
        public async Task<ActionResult<IEnumerable<CarteiraCambialDTO>>> ListarDadosDeCarteirasDeCambio()
        {
            IEnumerable<CarteiraCambialDTO> listCarteiras = await _exchangeWalletSvc.GetExchangeWallets();

            return Ok(listCarteiras);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<CarteiraCambialDTO>), StatusCodes.Status200OK)]
        [Route("ComprarMoeda")]        
        public async Task<ActionResult<string>> ComprarMoeda(decimal valorCompra, string siglaMoeda)
        {
            DadosCotacaoDTO cotacaoMoedaDeDestino;
            decimal valorConvertidoNaMoedaDeOrigem;
            String resultadoSaldo = await _walletSvc.GetValueInWalletAsync();
            
            decimal saldoAtualNaCarteiraPrincipal = Convert.ToDecimal(resultadoSaldo.Replace('.', ','));
                       
            if (siglaMoeda.Equals(QuotationHttpService.SIGLA_DOLAR))            
                cotacaoMoedaDeDestino = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_DOLAR);
            else if (siglaMoeda.Equals(QuotationHttpService.SIGLA_EURO))
                cotacaoMoedaDeDestino = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_EURO);
            else if (siglaMoeda.Equals(QuotationHttpService.SIGLA_REAL))
                cotacaoMoedaDeDestino = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_REAL);
            else
                return BadRequest("Sigla de moeda inválida");

            valorConvertidoNaMoedaDeOrigem = Convert.ToDecimal(cotacaoMoedaDeDestino.converted) * valorCompra;

            if (saldoAtualNaCarteiraPrincipal < valorConvertidoNaMoedaDeOrigem)
                return BadRequest("Não há saldo suficiente na carteira principal para realizar a Compra.");

            HttpResponseMessage responseRetirarDinheiro = await _walletSvc.TakeMoneyfromWallet(valorConvertidoNaMoedaDeOrigem, string.Format("Compra de moeda {0} ", siglaMoeda));

            if (responseRetirarDinheiro.StatusCode != HttpStatusCode.OK)
                return StatusCode(500, "Erro ao tentar retirar dinheiro da carteira principal venda.");

            HttpResponseMessage responseReceberDinheiroDeCambio = await _exchangeWalletSvc.ReceiveMoney(valorCompra, siglaMoeda);

            if (responseReceberDinheiroDeCambio.StatusCode != HttpStatusCode.OK)
                return StatusCode(500, "Erro ao tentar Receber valor em carteira de Câmbio.");

            return Ok("Compra realizada com sucesso");
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<CarteiraCambialDTO>), StatusCodes.Status200OK)]
        [Route("VenderMoeda")]
        public async Task<ActionResult<string>> VenderMoeda(decimal valorVenda, string siglaMoeda)
        {
            DadosCotacaoDTO cotacaoMoedaVendida;
            decimal valorConvertidoParaMoedaDaCarteiraPrincipal;
            decimal saldoNaCarteiraDeOrigem = 0;
            string moedaVendida;

            IEnumerable<CarteiraCambialDTO> listCarteiras = await _exchangeWalletSvc.GetExchangeWallets();

            if (siglaMoeda.Equals(QuotationHttpService.SIGLA_DOLAR))
            {
                saldoNaCarteiraDeOrigem =
                    Convert.ToDecimal(
                        listCarteiras.Where(c => c.siglaDaMoeda.Equals(QuotationHttpService.SIGLA_DOLAR)).FirstOrDefault().valorNacarteira.Replace('.', ','));

                cotacaoMoedaVendida = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_DOLAR);

                moedaVendida = QuotationHttpService.NOME_DOLAR;
            }
            else if (siglaMoeda.Equals(QuotationHttpService.SIGLA_EURO))
            {
                saldoNaCarteiraDeOrigem =
                    Convert.ToDecimal(
                        listCarteiras.Where(c => c.siglaDaMoeda.Equals(QuotationHttpService.SIGLA_EURO)).FirstOrDefault().valorNacarteira.Replace('.', ','));

                cotacaoMoedaVendida = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_EURO);
                moedaVendida = QuotationHttpService.NOME_EURO;
            }
            else if (siglaMoeda.Equals(QuotationHttpService.SIGLA_REAL))
            {
                saldoNaCarteiraDeOrigem =
                    Convert.ToDecimal(
                        listCarteiras.Where(c => c.siglaDaMoeda.Equals(QuotationHttpService.SIGLA_REAL)).FirstOrDefault().valorNacarteira.Replace('.', ','));

                cotacaoMoedaVendida = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_REAL);
                moedaVendida = QuotationHttpService.NOME_REAL;
            }
            else
                return BadRequest("Sigla de moeda inválida");
                        
            valorConvertidoParaMoedaDaCarteiraPrincipal = Convert.ToDecimal(cotacaoMoedaVendida.converted) * valorVenda;

            if (saldoNaCarteiraDeOrigem < valorConvertidoParaMoedaDaCarteiraPrincipal)
                return BadRequest(string.Format("Não há saldo suficiente na carteira de {0} para realizar a venda.", moedaVendida));

            HttpResponseMessage responseVenda = await _exchangeWalletSvc.SellMoney(valorVenda,siglaMoeda);

            if(responseVenda.StatusCode != HttpStatusCode.OK)
                return StatusCode(500, "Erro ao tentar realizar venda.");

            HttpResponseMessage ResponseColocaDinheiro = await _walletSvc.PutMoneyInWallet(valorConvertidoParaMoedaDaCarteiraPrincipal, string.Format("venda de {0}", moedaVendida));

            if (ResponseColocaDinheiro.StatusCode != HttpStatusCode.OK)
                return StatusCode(500, "Erro ao tentar realizar venda.");

            return Ok("Venda realizada com sucesso");
        }
    }
}
