using HRISAPI.Application.DTO.User;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace HRISAPI.Application.Services 
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleService(RoleManager<IdentityRole> roleManager,UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<Response> CreateRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                return new Response { Status = "Success", Message = "Role created successfully!" };
            }
            return new Response
            {
                Status = "Error",
                Message = "Role is already exist."
            };
        }
        public async Task<Response> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return new Response
                {
                    Status = "Error",
                    Message = "Role does not exist."
                };
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return new Response
                {
                    Status = "Success",
                    Message = "Role deleted successfully!"
                };
            }
            else
            {
                return new Response
                {
                    Status = "Error",
                    Message = "Error occurred while deleting the role."
                };
            }
        }
        public async Task<Response> AssignRoleAsync(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Response { Status = "Error", Message = "Role does not exist." };
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null){
                return new Response { Status = "Error", Message = "User not found." };
            }
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists){
                return new Response { Status = "Error", Message = "Role does not exist."};
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded){
                return new Response { Status = "Success", Message = "Role assigned successfully."};
            }
            return new Response { Status = "Error", Message = "Failed to assign role." };
        }
        public async Task<Response> ModifyUserRolesAsync(string userId, List<string> rolesToAdd, List<string> rolesToRemove)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null){
                return new Response { Status = "Error", Message = "User not found." };
            }
            if (rolesToRemove != null && rolesToRemove.Count > 0){
                var resultRemove = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!resultRemove.Succeeded){
                    return new Response { Status = "Error", Message = "Failed to remove roles." };
                }
            }
            if (rolesToAdd != null && rolesToAdd.Count()>0){
                var resultAdd = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!resultAdd.Succeeded){
                    return new Response { Status = "Error", Message = "Failed to add roles." };
                }
            }
            return new Response { Status = "Success", Message = "User roles modified successfully." };
        }
        public async Task<Response> RevokeRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null){
                return new Response { Status = "Error", Message = "User not found." };
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded){
                return new Response { Status = "Success", Message = "Role revoked successfully." };
            }
            return new Response { Status = "Error", Message = "Failed to revoke role." };
        }
    }
}