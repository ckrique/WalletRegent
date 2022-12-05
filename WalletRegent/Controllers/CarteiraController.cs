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
    }
}
