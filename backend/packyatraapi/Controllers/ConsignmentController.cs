using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsignmentController : ControllerBase
    {
        private readonly IConsignmentService _consignmentService;

        public ConsignmentController(IConsignmentService consignmentService)
        {
            _consignmentService = consignmentService;
        }

        // POST: api/Consignment/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateConsignment([FromBody] GoodsConsignmentNote model)
        {
            try
            {
                model.GeneratedDate = DateTime.UtcNow;
                var result = await _consignmentService.CreateAsync(model);

                return Ok(new ApiResponse<GoodsConsignmentNote>
                {
                    Success = true,
                    Data = result,
                    Message = "Goods consignment note created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<GoodsConsignmentNote>
                {
                    Success = false,
                    Message = $"Error creating consignment: {ex.Message}"
                });
            }
        }

        // GET: api/Consignment/gc/{gcNumber}
        [HttpGet("gc/{gcNumber}")]
        public async Task<IActionResult> GetByGcNumber(string gcNumber)
        {
            try
            {
                var data = await _consignmentService.GetByGcNumberAsync(gcNumber);

                if (data == null)
                {
                    return NotFound(new ApiResponse<GoodsConsignmentNote>
                    {
                        Success = false,
                        Message = "GC Number not found"
                    });
                }

                return Ok(new ApiResponse<GoodsConsignmentNote>
                {
                    Success = true,
                    Data = data,
                    Message = "Consignment fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<GoodsConsignmentNote>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // GET: api/Consignment/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _consignmentService.GetAllAsync();

                return Ok(new ApiResponse<List<GoodsConsignmentNote>>
                {
                    Success = true,
                    Data = list,
                    Message = "All consignments fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<GoodsConsignmentNote>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // DELETE: api/Consignment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _consignmentService.DeleteAsync(id);

                return Ok(new ApiResponse<bool>
                {
                    Success = deleted,
                    Data = deleted,
                    Message = deleted ? "Consignment deleted successfully" : "Delete failed"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
