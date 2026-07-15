using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly AdminUserService _adminUserService;

        public AdminUserController(AdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminUserService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserAdmin user)
        {
            try
            {
                var createdUser = await _adminUserService.CreateUserAsync(user);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to create user", Error = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserAdmin user)
        {
            if (userId != user.UserId) return BadRequest("User ID mismatch");
            var success = await _adminUserService.UpdateUserAsync(user);
            return success ? Ok("User updated successfully") : NotFound("User not found");
        }

        [HttpDelete]
        [Route("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var success = await _adminUserService.DeleteUserAsync(userId);
            return success ? Ok("User deleted successfully") : NotFound("User not found");
        }
    }
}