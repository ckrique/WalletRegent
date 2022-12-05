namespace WalletRegent.Services
{
    public interface IWalletHttpService
    {
        public Task<string> GetValueInWalletAsync();

        public Task<HttpResponseMessage> PutMoneyInWallet(decimal value, string reason);

        public Task<HttpResponseMessage> TakeMoneyfromWallet(decimal value, string reason);
    }
}
