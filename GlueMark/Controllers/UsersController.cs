using Application.DTOs.Users;
using Application.UseCases.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CreateUsers _createUser;
        private readonly GetAllUsers _getAllUsers;
        private readonly GetByIdUsers _getByIdUsers;
        private readonly GetByIdUsersSetting _getByIdUsersSetting;
        private readonly GetExistsCodeUsers _getExistsCodeUsers;
        private readonly UpdateUsers _updateUsers;
        private readonly UpdateSettingUsers _updateSettingUsers;
        private readonly PatchUsersStatus _patchUserStatus;
        private readonly UpdatePasswordChange _updatePasswordChange;

        public UsersController(CreateUsers createUser, GetAllUsers getAllUsers, GetByIdUsers getByIdUsers, UpdateUsers updateUsers, PatchUsersStatus patchUserStatus, GetExistsCodeUsers getExistsCodeUsers, UpdatePasswordChange updatePasswordChange, GetByIdUsersSetting getByIdUsersSetting, UpdateSettingUsers updateSettingUsers)
        {
            _createUser = createUser;
            _getAllUsers = getAllUsers;
            _getByIdUsers = getByIdUsers;
            _updateUsers = updateUsers;
            _patchUserStatus = patchUserStatus;
            _getExistsCodeUsers = getExistsCodeUsers;
            _updatePasswordChange = updatePasswordChange;
            _getByIdUsersSetting = getByIdUsersSetting;
            _updateSettingUsers = updateSettingUsers;
        }

        [HttpPost]
        [Route("UsersCreate")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] UsersCreateDto dto)
        {
            var result = await _createUser.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("UsersList")]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        { 
            var result = await _getAllUsers.ExecuteAsync(business_id,search, page,pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron usuarios." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }
        [HttpGet]
        [Route("UsersIdList")]
        [Authorize]
        public async Task<IActionResult> GetListId([FromQuery] int usersId)
        {
            var result = await _getByIdUsers.ExecuteAsync(usersId);
            if (result == null)
                return NotFound(new { message = "No se encontraron usuarios." });
            return Ok(result);
        }
        [HttpGet]
        [Route("UsersSettingById")]
        [Authorize]
        public async Task<IActionResult> GetSettingId([FromQuery] int usersId)
        {
            var result = await _getByIdUsersSetting.ExecuteAsync(usersId);
            if (result == null)
                return NotFound(new { message = "No se encontraron usuarios." });
            return Ok(result);
        }
        [HttpGet]
        [Route("UsersExistsCode")]
        [Authorize]
        public async Task<IActionResult> GetExistsCode([FromQuery] string code, [FromQuery] int businessId)
        {
            var result = await _getExistsCodeUsers.ExecuteAsync(code, businessId);
            return Ok(result);
        }
        [HttpPut]
        [Route("UsersUpdate")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UsersUpdateDto dto)
        {
            var result = await _updateUsers.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPut]
        [Route("UsersSettingUpdate")]
        [Authorize]
        public async Task<IActionResult> UpdateSetting([FromBody] UsersSettingUpdateDto dto)
        {
            var result = await _updateSettingUsers.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPut]
        [Route("UsersUpdatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UsersPasswordChangeDto dto)
        {
            var result = await _updatePasswordChange.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("UsersStatus")]
        [Authorize]
        public async Task<IActionResult> PatchStatus([FromBody] UsersStatusToggleDto dto)
        {
            var result = await _patchUserStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
