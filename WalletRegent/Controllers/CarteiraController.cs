using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using WalletRegent.DTO;
using WalletRegent.Services;

namespace WalletRegent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarteiraController : ControllerBase
    {
        private IWalletHttpService _walletSvc;

        public CarteiraController(IWalletHttpService walletSvc) => _walletSvc = walletSvc;

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("ObterSaldoEmCarteira")]
        public async Task<string> ObterSaldoEmCarteira()
        {
            String resultadoSaldo = await _walletSvc.GetValueInWalletAsync();
            //TODO:TRATAR QUANTIDADE DE CASAS DECIMAIS DO SALDO
            return JsonConvert.SerializeObject(resultadoSaldo);
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("ColocarDinheiroNaCarteira")]
        public async Task<ActionResult<string>> ColocarDinheiroNaCarteira(decimal valor, string fonte)
        {
            HttpResponseMessage response = await _walletSvc.PutMoneyInWallet(valor, fonte);

            if (response.StatusCode != HttpStatusCode.OK)
                return StatusCode(500, "Erro ao tentar Creditar valor na carteira de Principal.");

            return Ok("valor creditado com sucesso");
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("RetirarDinheiroNaCarteira")]
        public async Task<ActionResult<string>> RetirarDinheiroNaCarteira(decimal valor, string fonte)
        {
            HttpResponseMessage response = await _walletSvc.TakeMoneyfromWallet(valor, fonte);

            if (response.StatusCode != HttpStatusCode.OK)
                return StatusCode(500, "Erro ao tentar Creditar valor na carteira de Principal.");

            return Ok("valor retirado com sucesso");
        }
    }
}
