namespace WalletRegent.DTO
{
    public class DadosCotacaoDTO
    {
        public string Base { get; set; }
        public string to { get; set; }
        public int amount { get; set; }
        public double converted { get; set; }
        public double rate { get; set; }
        public int lastUpdate { get; set; }


        public string GetCotacao(string nomeMoeda)
        {
            string cotacao = converted.ToString("C");
            return string.Format("A cotação atual do {0} é de {1} Reais", nomeMoeda, cotacao);
        }
    }

   
}
