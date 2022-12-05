using WalletRegent.DTO;
using static WalletRegent.Services.QuotationHttpService;

namespace WalletRegent.Services
{
    public interface IQuotationHttpService
    {
        public Task<DadosCotacaoDTO> GetQuotation(string url);
    }
}
