using WalletRegent.DTO;

namespace WalletRegent.Services
{
    public interface IExchangeWalletHttpService
    {
        public Task<IEnumerable<CarteiraCambialDTO>> GetExchangeWallets();
        public Task<HttpResponseMessage> ReceiveMoney(decimal value, string currencyAcronym);
        public Task<HttpResponseMessage> SellMoney(decimal value, string currencyAcronym);
    }
}
