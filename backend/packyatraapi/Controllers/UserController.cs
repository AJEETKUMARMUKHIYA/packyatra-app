using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using Twilio.Rest.Preview.Wireless.Sim;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult>  CreateUser([FromBody] UserInfo userinfo)
        {
            // Logic to handle user creation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
           
            Address address = new Address();
            address.AddressID = userinfo.Address.AddressID;
            address.FromAddress=userinfo.Address.FromAddress;   
            address.ToAddress=userinfo.Address.ToAddress;
            var createdAddress = await _userService.AddAddressAsync(address);

            Users userval = new Users();
            userval.Email = userinfo.Email;
            userval.Name = userinfo.Name;
            userval.Password = userinfo.Password;
            userval.PhoneNumber = userinfo.PhoneNumber;
            userval.UserID = userinfo.UserID;
            userval.AddressID=createdAddress.AddressID;
            var createdUser = await _userService.CreateUserAsync(userval);
            //return CreatedAtAction(nameof(GetUserById), new { UserID = createdUser.UserID }, createdUser);
            return Ok(new { userID= createdUser.UserID,addressID= createdAddress.AddressID, message = "User created successfully!" });
        }

        // ✅ NEW: Update User Name and Email
        [HttpPut]
        [Route("Update/{userId}")]
      
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] Users updateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // ✅ Use the userId from route parameter, NOT from body
                var existingUser = await _userService.GetUserByIdAsync(userId);
                if (existingUser == null)
                    return NotFound($"User with ID {userId} not found.");

                // Update fields
                existingUser.Name = updateRequest.Name ?? existingUser.Name;
                existingUser.Email = updateRequest.Email ?? existingUser.Email;
                existingUser.PhoneNumber = updateRequest.PhoneNumber ?? existingUser.PhoneNumber;

                // Only update password if provided
                if (!string.IsNullOrEmpty(updateRequest.Password))
                    existingUser.Password = updateRequest.Password;

                var updated = await _userService.UpdateUserAsync(existingUser);

                if (updated)
                    return Ok(new
                    {
                        message = "User updated successfully!",
                        userId = existingUser.UserID,
                        name = existingUser.Name,
                        email = existingUser.Email
                    });
                else
                    return StatusCode(500, "Failed to update user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        // ✅ API to Check if a User Exists by Mobile Number
        [HttpGet("CheckUser/{mobileNumber}")]
        public async Task<IActionResult> CheckUserExists(string mobileNumber)
        {
            var user = await _userService.GetUserByMobileNumberAsync(mobileNumber);
            //if (user == null)
            //{
            //    return NotFound("User not found");
            //}
            return Ok(user);
        }

       

    }
}
