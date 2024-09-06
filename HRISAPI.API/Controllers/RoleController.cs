using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        //[Authorize(Roles = Roles.Role_Administrator)]
        [HttpPatch("/create_role")]
        public async Task<IActionResult> CreateRoleAsync([FromBody]string roleName)
        {
            var result = await _roleService.CreateRoleAsync(roleName);

            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpDelete("/delete_role")]
        public async Task<IActionResult> DeleteRoleAsync([FromBody]string roleName)
        {
            var result = await _roleService.DeleteRoleAsync(roleName);

            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);
        }
        //[Authorize(Roles = Roles.Role_Administrator)]
        [HttpPatch("/assign_role/{userId}")]
        public async Task<IActionResult> AssignRoleAsync(string userId,[FromBody] string roleName)
        {
            var result = await _roleService.AssignRoleAsync(userId,roleName);

            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpPatch("/modify_role/{userId}")]
        public async Task<IActionResult> ModifyUserRolesAsync([FromBody] ModifyUserRole modifyUserRole,string userId)
        {
            var result = await _roleService.ModifyUserRolesAsync(userId, modifyUserRole.RolesToAdd, modifyUserRole.RolesToRemove);

            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);

        }
        //[Authorize(Roles = Roles.Role_Administrator)]
        [HttpPatch("/remove_role/{userId}")]
        public async Task<IActionResult> RevokeRoleAsync(string userId,[FromBody] string roleName)
        {
            var result = await _roleService.RevokeRoleAsync(userId, roleName);

            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);
        }
    }

}