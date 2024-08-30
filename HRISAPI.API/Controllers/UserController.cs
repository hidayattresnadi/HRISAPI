using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _authService;
        public UserController(IUserService authService)
        {
            _authService = authService;
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpPost("register")]

        public async Task<IActionResult> RegisterAsync([FromBody] Register model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Register(model);

            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpGet("/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var user = await _authService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpPatch("/{id}")]
        public async Task<IActionResult> UpdateUserAsync(string userId, [FromBody] UpdateUserDTO updateUserData)
        {
            var user = await _authService.UpdateUser(userId,updateUserData);
            return Ok(user);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpDelete("/{id}")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            var user = await _authService.DeleteUser(userId);
            return Ok(user);
        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginAsync([FromBody] Login model)
        {
            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            var result = await _authService.Login(model);

            if (result.Status == "Error")

                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("refresh-Token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            var result = await _authService.RefreshToken(request);

            if (result.Status == "Error")

                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPatch("Log-Out")]
        public async Task<IActionResult> LogoutAsync([FromBody]string email)
        {

            var result = await _authService.LogoutAsync(email);

            if (result.Status == "Error")

                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}