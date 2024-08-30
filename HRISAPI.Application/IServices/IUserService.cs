using HRISAPI.Application.DTO.User;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.IServices
{
    public interface IUserService
    {
        Task<Response> Register(Register registerData);
        Task<Response> DeleteUser(string userId);
        Task<Response> UpdateUser(string userId, UpdateUserDTO updateUserData);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(string userId);
        Task<Response> Login(Login loginData);
        Task<Response> RefreshToken(RefreshTokenRequest request);
        Task<Response> LogoutAsync(string email);
    }
}
