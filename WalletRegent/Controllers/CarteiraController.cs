using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

            return JsonConvert.SerializeObject(resultadoSaldo);
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("ColocarDinheiroNaCarteira")]
        public async Task<string> ColocarDinheiroNaCarteira(decimal valor, string fonte)
        {
            _walletSvc.PutMoneyInWallet(valor, fonte);

            String resultadoSaldo = await _walletSvc.GetValueInWalletAsync();

            return JsonConvert.SerializeObject(resultadoSaldo);
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("RetirarDinheiroNaCarteira")]
        public async Task<string> RetirarDinheiroNaCarteira(decimal valor, string fonte)
        {
            _walletSvc.TakeMoneyfromWallet(valor, fonte);

            String resultadoSaldo = await _walletSvc.GetValueInWalletAsync();

            return JsonConvert.SerializeObject(resultadoSaldo);
        }
    }
}
