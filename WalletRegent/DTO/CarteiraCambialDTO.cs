using Newtonsoft.Json;

namespace WalletRegent.DTO
{
    public class CarteiraCambialDTO
    {        
        [JsonProperty("nomeMoeda")]
        public string nomeCarteira { get; set; }
        [JsonProperty("siglaMoeda")]
        public string siglaDaMoeda { get; set; }
        [JsonProperty("valor")]
        public string valorNacarteira { get; set;}
    }
}
