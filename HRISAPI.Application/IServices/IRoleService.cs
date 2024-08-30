using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRISAPI.Application.DTO.User;

namespace HRISAPI.Application.IServices
{
    public interface IRoleService
    {
        Task<Response> CreateRoleAsync(string roleName);
        Task<Response> DeleteRoleAsync(string roleName);
        Task<Response> AssignRoleAsync(string userId, string roleName);
        Task<Response> ModifyUserRolesAsync(string userId, List<string> rolesToAdd, List<string> rolesToRemove);
        Task<Response> RevokeRoleAsync(string userId, string roleName);
    }
}
