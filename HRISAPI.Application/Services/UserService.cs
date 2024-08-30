using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<AppUser> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }
        public async Task<Response> Register(Register model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);

            if (userExists != null)

                return new Response { Status = "Error", Message = "Email already exists!" };

            AppUser user = new AppUser()

            {
                Email = model.Email,

                SecurityStamp = Guid.NewGuid().ToString(),

                UserName = model.UserName,
                EmployeeId = model.EmployeeId
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)

                return new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                };
                
            var defaultRole = Roles.Role_Employee;
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(defaultRole));
            }    

            await _userManager.AddToRoleAsync(user, Roles.Role_Employee);

            return new Response
            {
                Status = "Success",
                Message = "User created successfully!"
            };
        }

        public async Task<Response> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Response { Status = "Error", Message = "User not found." };
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new Response { Status = "Error", Message = $"Failed to delete user: {errors}" };
            }
            return new Response { Status = "Success", Message = "User deleted successfully." };
        }

        public async Task<Response> UpdateUser(string userId, UpdateUserDTO updateUserData)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Response { Status = "Error", Message = "User not found." };
            }
            user.UserName = updateUserData.UserName;
            user.Email = updateUserData.Email;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new Response { Status = "Error", Message = $"Failed to update user: {errors}" };
            }
            return new Response { Status = "Success", Message = "User updated successfully." };
        }
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userDtos = users.Select(user => new UserDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            }).ToList();
            return userDtos;
        }
        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User is not found"); 
            }
            var userDto = new UserDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            };
            return userDto;
        }
        public async Task<Response> Login(Login loginData)
        {
            var user = await _userManager.FindByEmailAsync(loginData.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginData.Password))
            {
                //if(user.RefreshToken == null)
                //{
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("EmployeeId", user.EmployeeId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // Generate new access token
                var newAccessToken = GenerateAccessToken(authClaims);
                // Generate a new refresh token.
                var refreshToken = GenerateRefreshToken();
                // Save the new refresh token with associated user information.
                user.RefreshToken = refreshToken.Token;
                user.RefreshTokenExpire = refreshToken.ExpiryDate;
                await _userManager.UpdateAsync(user);
                return new AuthLoginResponse
                {
                    Message = "Login Success",
                    Status = "Success",
                    ExpiredOn = newAccessToken.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpireOn = refreshToken.ExpiryDate
                };
                //else
                //{
                //    return new AuthLoginResponse
                //    {
                //        RefreshToken = user.RefreshToken,
                //        RefreshTokenExpireOn = user.RefreshTokenExpire.Value
                //    };
                //}
            }
            return new Response { Status = "Error", Message = "Password not valid!" };
        }
        public async Task<Response> RefreshToken(RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return new Response { Status = "Error", Message = "Invalid Request" };
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user == null)
            {
                return new Response { Status = "Error", Message = "Invalid Token" };
            }
            if (user.RefreshTokenExpire < DateTime.UtcNow)
            {
                user.RefreshTokenExpire = null;
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
                return new Response { Status = "Error", Message = "Refresh token expired. Please log in again." };
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName), // Nama pengguna
                new Claim("EmployeeId", user.EmployeeId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID unik untuk token
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Generate new access token
            var newAccessToken = GenerateAccessToken(authClaims);

            return new AuthLoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                ExpiredOn = newAccessToken.ValidTo,
                RefreshTokenExpireOn = user.RefreshTokenExpire.Value,
                RefreshToken = request.RefreshToken,
                Status = "Success",
                Message = "Here is the new access token"
            };
        }
        public async Task<Response> LogoutAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Response { Status = "Error", Message = "user is not found" };
            }
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpire = null;
                await _userManager.UpdateAsync(user);
            }
            return new Response
            {
                Message = "Success Logout",
                Status = "Success",
            };
        }
        // Generates a new refresh token using a cryptographic random number generator.
        private static RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];  // Prepare a buffer to hold the random bytes.
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);  // Fill the buffer with cryptographically strong random bytes.

                var token = Convert.ToBase64String(randomNumber); // Convert the bytes to a Base64 string and return.
                return new RefreshToken
                {
                    Token = token,
                    ExpiryDate = DateTime.UtcNow.AddDays(7)
                };
            }
        }
        private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddHours(3), // Set waktu kadaluarsa yang sesuai
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }
    }
}
