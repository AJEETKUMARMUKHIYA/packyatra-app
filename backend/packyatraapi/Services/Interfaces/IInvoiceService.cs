using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> GenerateInvoiceAsync(string quotationNumber);
        Task<InvoiceDetailsResponse> GetInvoiceDetailsByNumberAsync(string invoiceNumber);

    }
}
