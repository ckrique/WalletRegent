using Microsoft.AspNetCore.Mvc;

using System.Net.Http;
using Newtonsoft.Json;
using WalletRegent.DTO;
using System.IO;
using System.Net.Http.Headers;
using WalletRegent.Services;
using System.Collections.Generic;

namespace WalletRegent.Controllers
{
    [Route("api/[controller]")]
    public class CambioController : ControllerBase
    {
        private IQuotationHttpService _quotationSvc;

        public CambioController(IQuotationHttpService catalogSvc) => _quotationSvc = catalogSvc;

        //public IActionResult Index()
        //{
        //    return View();
        //}

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

        // GET api/cambio/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
