using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface IConsignmentService
    {
        Task<GoodsConsignmentNote> CreateAsync(GoodsConsignmentNote model);
        Task<GoodsConsignmentNote> GetByGcNumberAsync(string gcNumber);
        Task<List<GoodsConsignmentNote>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
    }
}
