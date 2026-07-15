using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AdminUserService _adminUserService;

        public AuthController(AdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "First name and password are required" });
            }

            var user = await _adminUserService.ValidateUserAsync(request.FirstName, request.Password);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid first name or password" });
            }

            // Return user details for frontend navigation
            return Ok(new
            {
                userId = user.UserId,
                roleId = user.RoleId,
                firstName = user.FirstName,
                lastName = user.LastName
            });
        }
    }

    // DTO for login request
    public class LoginRequest
    {
        public string FirstName { get; set; } // Changed from Username to FirstName
        public string Password { get; set; }
    }
}