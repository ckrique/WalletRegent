namespace WalletRegent.Services
{
    public interface IWalletHttpService
    {
        public Task<string> GetValueInWalletAsync();
    }
}
