using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Services;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _authService;
        private readonly UserManager<AppUser> _userManager;
        public UserController(IUserService authService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
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
        //[Authorize(Roles = Roles.Role_Administrator)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }
        //[Authorize(Roles = Roles.Role_Administrator)]
        [HttpGet("/{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var user = await _authService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpPatch("/{userId}")]
        public async Task<IActionResult> UpdateUserAsync(string userId, [FromBody] UpdateUserDTO updateUserData)
        {
            var user = await _authService.UpdateUser(userId,updateUserData);
            return Ok(user);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpDelete("/{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var user = await _authService.DeleteUser(id);
            return Ok(user);
        }

        [HttpPost("login")]

        public async Task<IActionResult> LoginAsync([FromBody] Login model)
        {
            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            var result = await _authService.Login(model);

            if (result.Status == "Error")
            {
                return BadRequest(result.Message);
            }
            else
            {
                var resultSuccess = result as AuthLoginResponse;
                SetRefreshTokenCookie("AuthToken", resultSuccess!.Token, resultSuccess.ExpiredOn);

                SetRefreshTokenCookie("RefreshToken", resultSuccess.RefreshToken,
                resultSuccess.RefreshTokenExpireOn);
            }
            return Ok(result);
        }
        private void SetRefreshTokenCookie(string tokenType, string? token, DateTime? expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // Hanya dapat diakses oleh server
                Secure = false,    // Hanya dikirim melalui HTTPS
                SameSite = SameSiteMode.None, // Cegah serangan CSRF
                Expires = expires // Waktu kadaluarsa token
            };

            Response.Cookies.Append(tokenType, token, cookieOptions);
        }
        [HttpPost("refresh-Token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            var result = await _authService.RefreshToken(refreshToken);

            if (result.Status == "Error")
            {
                return BadRequest(result.Message);
            }
            else
            {
                var resultSuccess = result as AuthLoginResponse;
                Console.WriteLine(resultSuccess);
                SetRefreshTokenCookie("AuthToken", resultSuccess.Token, resultSuccess.ExpiredOn);
                //SetRefreshTokenCookie("RefreshToken", result.RefreshToken, result.RefreshTokenExpiration);
            }





            return Ok(result);
        }

        [Authorize]
        [HttpPatch("Log-Out")]
        public async Task<IActionResult> LogoutAsync([FromQuery]string email)
        {

            var result = await _authService.LogoutAsync(email);

            if (result.Status == "Error")
            {
                return BadRequest(result.Message);
            }

            Response.Cookies.Delete("AuthToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(result);
        }

        [HttpPost("/reset_password")]
        public async Task<IActionResult> ResetPasswordByAdmin(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Hapus password lama (jika ada)
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return BadRequest(removePasswordResult.Errors);
            }

            // Tambahkan password baru
            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (addPasswordResult.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }
            else
            {
                return BadRequest(addPasswordResult.Errors);
            }
        }
    }
}