using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly PriceService _priceService;

        public PriceController(PriceService priceService)
        {
            _priceService = priceService;
        }
        [HttpGet]
        [Route("GetPrice")]
        public async Task<IActionResult> GetPrice([FromQuery] int distance, [FromQuery] int cftTotal, [FromQuery] string activeTab)
        {
            var price = await _priceService.GetPriceAsync(distance, cftTotal,activeTab);
            return price.HasValue ? Ok(new { price }) : NotFound("No matching price found");
        }
        //public async Task<IActionResult> GetPrice()
        //{
        //    var price = await _priceService.GetPriceAsync(15, 120);
        //    return price.HasValue ? Ok(new { price }) : NotFound("No matching price found");
        //}

    }


}
