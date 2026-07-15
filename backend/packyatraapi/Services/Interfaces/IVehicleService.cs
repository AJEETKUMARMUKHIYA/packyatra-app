using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<List<Vehicle>> GetAvailableVehiclesAsync();
        Task<Vehicle> CreateVehicleAsync(VehicleCreateDto dto);
        Task<Vehicle> UpdateVehicleAsync(int id, VehicleUpdateDto dto);
        Task<bool> DeleteVehicleAsync(int id);
        Task<bool> AssignVehicleAsync(AssignVehicleDto dto);
        Task<bool> UnassignVehicleAsync(int bookingId, int vehicleId);
    }
}
