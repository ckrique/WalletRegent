using System.Net.Http;

namespace WalletRegent.Services
{
    public class WalletHttpService : IWalletHttpService
    {
        private readonly HttpClient _httpClient;        
        private readonly string _serviceBaseUrl = "https://5b91-2804-14d-5c73-6ee5-74b8-9d1c-9885-2ee.sa.ngrok.io";
        private readonly string _path = "/api/v1/CarteiraFinanceira/ListarSaldo";
        private readonly string _apiKey = "glmlf0inrekl0h9clleiron6bv04dngp9jbuogjst82lfe00hcc48";

        public WalletHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetValueInWalletAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _path;

            var responseString = await _httpClient.GetStringAsync(uri);

            return responseString;
        }
    }
}
