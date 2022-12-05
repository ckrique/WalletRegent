using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WalletRegent.DTO;

namespace WalletRegent.Services
{
    public class ExchangeWalletHttpService : IExchangeWalletHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceBaseUrl = "https://0a96-2804-14d-5c73-6ee5-74b8-9d1c-9885-2ee.sa.ngrok.io";
        private readonly string _pathListAccountsBalances= "/api/v1/AtivosCambiais/ListarSaldos";
        private readonly string _pathReceiveMoney = "/api/v1/AtivosCambiais/ReceberCompraDeMoeda";
        private readonly string _pathSellCurrency = "/api/v1/AtivosCambiais/RealizarVendaDeMoeda";

        public ExchangeWalletHttpService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CarteiraCambialDTO>> GetExchangeWallets()
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _pathListAccountsBalances;            

            var responseString = await _httpClient.GetStringAsync(uri);

            IEnumerable<CarteiraCambialDTO> listExchangeWallets = JsonConvert.DeserializeObject<IList<CarteiraCambialDTO>>(responseString);

            return listExchangeWallets;
        }

        public async Task<HttpResponseMessage> ReceiveMoney(decimal value, string currencyAcronym)
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _pathReceiveMoney;

            AtivoCambialDTO exchangeAsset = new AtivoCambialDTO();

            exchangeAsset.valor = value;
            exchangeAsset.siglaMoeda = currencyAcronym;

            var content = new StringContent(JsonConvert.SerializeObject(exchangeAsset).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await _httpClient.PostAsync(uri, content);

            return result;
        }

        public async Task<HttpResponseMessage> SellMoney(decimal value, string currencyAcronym)
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warnin", "40");

            var uri = _serviceBaseUrl + _pathSellCurrency;

            AtivoCambialDTO exchangeAssetSold = new AtivoCambialDTO();

            exchangeAssetSold.valor = value;
            exchangeAssetSold.siglaMoeda = currencyAcronym;

            var content = new StringContent(JsonConvert.SerializeObject(exchangeAssetSold).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage result = await _httpClient.PostAsync(uri, content);
            
            return result;
        }
    }
}
