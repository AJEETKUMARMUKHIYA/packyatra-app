using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
        public class VehicleController : ControllerBase
        {
            private readonly IVehicleService _vehicleService;

            public VehicleController(IVehicleService vehicleService)
            {
                _vehicleService = vehicleService;
            }

            // ✅ GET: api/vehicle/all
            [HttpGet("all")]
            public async Task<IActionResult> GetAll()
            {
                try
                {
                    var data = await _vehicleService.GetAllVehiclesAsync();

                    return Ok(new ApiResponse<List<Vehicle>>
                    {
                        Success = true,
                        Data = data,
                        Message = "Vehicles fetched successfully"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse<List<Vehicle>>
                    {
                        Success = false,
                        Data = null,
                        Message = $"Error fetching vehicles: {ex.Message}"
                    });
                }
            }

            // ✅ GET: api/vehicle/available
            [HttpGet("available")]
            public async Task<IActionResult> GetAvailable()
            {
                try
                {
                    var data = await _vehicleService.GetAvailableVehiclesAsync();

                    return Ok(new ApiResponse<List<Vehicle>>
                    {
                        Success = true,
                        Data = data,
                        Message = "Available vehicles fetched successfully"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse<List<Vehicle>>
                    {
                        Success = false,
                        Data = null,
                        Message = $"Error fetching available vehicles: {ex.Message}"
                    });
                }
            }

            // ✅ GET: api/vehicle/{id}
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                try
                {
                    var data = await _vehicleService.GetVehicleByIdAsync(id);

                    return Ok(new ApiResponse<Vehicle>
                    {
                        Success = true,
                        Data = data,
                        Message = "Vehicle fetched successfully"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse<Vehicle>
                    {
                        Success = false,
                        Data = null,
                        Message = $"Error fetching vehicle: {ex.Message}"
                    });
                }
            }

            // ✅ POST: api/vehicle
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] VehicleCreateDto dto)
            {
                try
                {
                    var data = await _vehicleService.CreateVehicleAsync(dto);

                    return Ok(new ApiResponse<Vehicle>
                    {
                        Success = true,
                        Data = data,
                        Message = "Vehicle created successfully"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiResponse<Vehicle>
                    {
                        Success = false,
                        Data = null,
                        Message = $"Error creating vehicle: {ex.Message}"
                    });
                }
            }

            // ✅ PUT: api/vehicle/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] VehicleUpdateDto dto)
            {
                try
                {
                    var data = await _vehicleService.UpdateVehicleAsync(id, dto);

                    return Ok(new ApiResponse<Vehicle>
                    {
                        Success = true,
                        Data = data,
                        Message = "Vehicle updated successfully"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse<Vehicle>
                    {
                        Success = false,
                        Data = null,
                        Message = $"Error updating vehicle: {ex.Message}"
                    });
                }
            }

            // ✅ DELETE: api/vehicle/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {
                    var result = await _vehicleService.DeleteVehicleAsync(id);

                    return Ok(new ApiResponse<bool>
                    {
                        Success = result,
                        Data = result,
                        Message = result ? "Vehicle deleted successfully" : "Vehicle not found"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = $"Error deleting vehicle: {ex.Message}"
                    });
                }
            }

            // ✅ POST: api/vehicle/assign
            [HttpPost("assign")]
            public async Task<IActionResult> AssignVehicle([FromBody] AssignVehicleDto dto)
            {
                try
                {
                    var result = await _vehicleService.AssignVehicleAsync(dto);

                    return Ok(new ApiResponse<bool>
                    {
                        Success = result,
                        Data = result,
                        Message = "Vehicle assigned successfully"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = $"Error assigning vehicle: {ex.Message}"
                    });
                }
            }
        // ✅ POST: api/vehicle/unassign
        [HttpPost("unassign")]
        public async Task<IActionResult> UnassignVehicle([FromBody] AssignVehicleDto dto)
        {
            try
            {
                var result = await _vehicleService.UnassignVehicleAsync(
                    dto.BookingId,
                    dto.VehicleId
                );

                return Ok(new ApiResponse<bool>
                {
                    Success = result,
                    Data = result,
                    Message = "Vehicle unassigned successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Error unassigning vehicle: {ex.Message}"
                });
            }
        }

    }
    }

