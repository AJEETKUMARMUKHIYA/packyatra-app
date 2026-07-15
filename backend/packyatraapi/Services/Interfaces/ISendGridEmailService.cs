using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface ISendGridEmailService
    {
        Task<bool> SendQuotationAsync(SendQuotationEmailDto dto);
    }
}
