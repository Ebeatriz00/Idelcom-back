using Application.DTOs.Profiles;
using Application.UseCases.Modules;
using Application.UseCases.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly CreateProfiles _createProfiles;
        private readonly GetAllProfiles _getAllProfiles;
        private readonly GetSelectProfiles _getSelectProfiles;
        private readonly GetByIdProfiles _getByIdProfiles;
        private readonly UpdateProfiles _updateProfiles;
        private readonly PatchProfilesStatus _patchProfilesStatus;

        public ProfilesController(CreateProfiles createProfiles, GetAllProfiles getAllProfiles, GetByIdProfiles getByIdProfiles, UpdateProfiles updateProfiles, PatchProfilesStatus patchProfilesStatus, GetSelectProfiles getSelectProfiles)
        {
            _createProfiles = createProfiles;
            _getAllProfiles = getAllProfiles;
            _getByIdProfiles = getByIdProfiles;
            _updateProfiles = updateProfiles;
            _patchProfilesStatus = patchProfilesStatus;
            _getSelectProfiles = getSelectProfiles;
        }

        [HttpPost]
        [Route("ProfilesCreate")]
        public async Task<IActionResult> Create([FromBody] ProfilesCreateDto dto)
        {
            var result = await _createProfiles.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProfilesList")]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllProfiles.ExecuteAsync(business_id, search, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron perfiles." });

            // Headers útiles para el frontend (ej. DataTables, React Table, etc.)
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }
        [HttpGet]
        [Route("ProfilesSelect")]
        public async Task<IActionResult> ModulesSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectProfiles.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProfilesIdList")]
        public async Task<IActionResult> GetListId([FromQuery] int ProfilesId)
        {
            var result = await _getByIdProfiles.ExecuteAsync(ProfilesId);
            if (result == null)
                return NotFound(new { message = "No se encontraron perfiles." });
            return Ok(result);
        }
        [HttpPut]
        [Route("ProfilesUpdate")]
        public async Task<IActionResult> Update([FromBody] ProfilesUpdateDto dto)
        {
            var result = await _updateProfiles.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPatch]
        [Route("ProfilesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ProfilesStatusToggleDto dto)
        {
            var result = await _patchProfilesStatus.ExecuteAsync(dto);
            return Ok(result);

        }
    }
}
