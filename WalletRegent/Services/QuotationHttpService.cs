using Newtonsoft.Json;
using WalletRegent.DTO;
using static System.Net.WebRequestMethods;

namespace WalletRegent.Services
{
    public class QuotationHttpService : IQuotationHttpService
    {
        private readonly HttpClient _httpClient;        
        private readonly string _serviceBaseUrl = "https://anyapi.io/api/v1/exchange/convert";
        private readonly string _apiKey = "glmlf0inrekl0h9clleiron6bv04dngp9jbuogjst82lfe00hcc48";

        public const string SIGLA_EURO = "EUR";
        public const string SIGLA_DOLAR = "USD";
        public const string SIGLA_REAL = "BRL";

        public const string NOME_EURO = "Euro";
        public const string NOME_DOLAR = "Dólar";
        public const string NOME_REAL = "Real";

        public QuotationHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DadosCotacaoDTO> GetQuotation(string siglaMoeda)
        {
            string complementoURL = string.Format("?base={0}&to=BRL&amount=1&apiKey={1}", siglaMoeda, _apiKey);

            var uri = _serviceBaseUrl + complementoURL;

            var responseString = await _httpClient.GetStringAsync(uri);

            DadosCotacaoDTO dadosCotacao = JsonConvert.DeserializeObject<DadosCotacaoDTO>(responseString);

            //long timestmp = 1669939200;
            //var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(1669939200).ToLocalTime();

            return dadosCotacao;
        }
    }
}
