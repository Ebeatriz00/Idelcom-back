using Application.DTOs.Users;
using Application.DTOs.UsersPreferences;
using Application.UseCases.Users;
using Application.UseCases.UsersPreferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersPreferencesController : ControllerBase
    {
        private readonly GetNotifByIdUsersPrefe _getNotifByIdUsersPrefe;
        private readonly GetPrefeByIdUsersPrefe _getPrefeByIdUsersPrefe;
        private readonly GetSeattingByIdUsersPrefe _getSeattingByIdUsersPrefe;
        private readonly UpdateNotiUsersPrefe _updateNotiUsersPrefe;
        private readonly UpdatePrefeUsersPrefe _updatePrefeUsersPrefe;
        private readonly UpdateSettingUsersPrefe _updateSettingUsersPrefe;

        public UsersPreferencesController(GetNotifByIdUsersPrefe getNotifByIdUsersPrefe, GetPrefeByIdUsersPrefe getPrefeByIdUsersPrefe, GetSeattingByIdUsersPrefe getSeattingByIdUsersPrefe, UpdateNotiUsersPrefe updateNotiUsersPrefe, UpdatePrefeUsersPrefe updatePrefeUsersPrefe, UpdateSettingUsersPrefe updateSettingUsersPrefe)
        {
            _getNotifByIdUsersPrefe = getNotifByIdUsersPrefe;
            _getPrefeByIdUsersPrefe = getPrefeByIdUsersPrefe;
            _getSeattingByIdUsersPrefe = getSeattingByIdUsersPrefe;
            _updateNotiUsersPrefe = updateNotiUsersPrefe;
            _updatePrefeUsersPrefe = updatePrefeUsersPrefe;
            _updateSettingUsersPrefe = updateSettingUsersPrefe;
        }

        [HttpGet]
        [Route("UsersNotifById")]
        public async Task<IActionResult> GetNotiId([FromQuery] long usersId, long businessId)
        {
            var result = await _getNotifByIdUsersPrefe.ExecuteAsync(usersId, businessId);
            if (result == null)
                return NotFound(new { message = "No se encontraron preferencias." });
            return Ok(result);
        }
        [HttpGet]
        [Route("UsersPrefeById")]
        public async Task<IActionResult> GetPrefeId([FromQuery] long usersId, long businessId)
        {
            var result = await _getPrefeByIdUsersPrefe.ExecuteAsync(usersId, businessId);
            if (result == null)
                return NotFound(new { message = "No se encontraron preferencias." });
            return Ok(result);
        }
        [HttpGet]
        [Route("UsersSettById")]
        public async Task<IActionResult> GetSettId([FromQuery] long usersId, long businessId)
        {
            var result = await _getSeattingByIdUsersPrefe.ExecuteAsync(usersId, businessId);
            if (result == null)
                return NotFound(new { message = "No se encontraron preferencias." });
            return Ok(result);
        }

        [HttpPut]
        [Route("UsersNotifUpdate")]
        //[Authorize]
        public async Task<IActionResult> UpdateNoti([FromBody] UsersPrefeNotiUpdateDto dto)
        {
            var result = await _updateNotiUsersPrefe.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Route("UsersPrefeUpdate")]
        //[Authorize]
        public async Task<IActionResult> UpdatePrefe([FromBody] UsersPrefeUpdateDto dto)
        {
            var result = await _updatePrefeUsersPrefe.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPut]
        [Route("UsersSettUpdate")]
        //[Authorize]
        public async Task<IActionResult> UpdateSett([FromBody] UsersPrefeSettingUpdateDto dto)
        {
            var result = await _updateSettingUsersPrefe.ExecuteAsync(dto);
            return Ok(result);
        }

    }
}
