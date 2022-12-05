namespace WalletRegent.Services
{
    public interface IWalletHttpService
    {
        public Task<string> GetValueInWalletAsync();

        public Task PutMoneyInWallet(decimal value, string reason);

        public Task TakeMoneyfromWallet(decimal value, string reason);
    }
}
