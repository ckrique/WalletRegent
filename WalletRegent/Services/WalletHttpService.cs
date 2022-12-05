using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using WalletRegent.DTO;
using System;

namespace WalletRegent.Services
{
    public class WalletHttpService : IWalletHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceBaseUrl = "https://ff20-2804-14d-5c73-6ee5-74b8-9d1c-9885-2ee.sa.ngrok.io";
        private readonly string _pathListarSaldo = "/api/v1/CarteiraFinanceira/ListarSaldo";
        private readonly string _pathPutMoney = "/api/v1/CarteiraFinanceira/ColocarDinheiro";
        private readonly string _pathTakeMoney = "/api/v1/CarteiraFinanceira/RetirarDinheiro";

        public WalletHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetValueInWalletAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _pathListarSaldo;

            var responseString = await _httpClient.GetStringAsync(uri);

            return responseString;
        }

        public async Task PutMoneyInWallet(decimal value, string reason)
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _pathPutMoney;

            MovimentoFinanceiroNaCarteira moneyMovement = new MovimentoFinanceiroNaCarteira();

            moneyMovement.valor = value;
            moneyMovement.descricaoDeFinalidade = reason;

            var content = new StringContent(JsonConvert.SerializeObject(moneyMovement).ToString(), Encoding.UTF8, "application/json");

            var result = _httpClient.PostAsync(uri, content).Result;
        }

        public async Task TakeMoneyfromWallet(decimal value, string reason)
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _pathTakeMoney;

            MovimentoFinanceiroNaCarteira moneyMovement = new MovimentoFinanceiroNaCarteira();

            moneyMovement.valor = value;
            moneyMovement.descricaoDeFinalidade = reason;

            var content = new StringContent(JsonConvert.SerializeObject(moneyMovement).ToString(), Encoding.UTF8, "application/json");

            var result = _httpClient.PostAsync(uri, content).Result;
        }
    }
}
