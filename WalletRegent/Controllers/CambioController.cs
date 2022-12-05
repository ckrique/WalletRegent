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
            else            
                cotacaoMoedaDeDestino = await _quotationSvc.GetQuotation(QuotationHttpService.SIGLA_REAL);

            valorConvertidoNaMoedaDeOrigem = Convert.ToDecimal(cotacaoMoedaDeDestino.converted) * valorCompra;

            if (saldoAtualNaCarteiraPrincipal < valorConvertidoNaMoedaDeOrigem)
                return BadRequest("Não há saldo suficiente na carteira principal para realizar a Compra.");

            await _walletSvc.TakeMoneyfromWallet(valorConvertidoNaMoedaDeOrigem, string.Format("Compra de moeda {0} ", siglaMoeda));

            var resultado = await _exchangeWalletSvc.ReceiveMoney(valorCompra, siglaMoeda);

            return Ok("Compra realizada com sucesso");
        }
    }
}
