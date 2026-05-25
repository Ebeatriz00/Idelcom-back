using Application.DTOs.ContactType;
using Application.UseCases.ContactType;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactTypeController : ControllerBase
    {
        private readonly CreateContactType _createContactType;
        private readonly GetAllContactType _getAllContactType;
        private readonly GetContactTypeById _getContactTypeById;
        private readonly UpdateContactType _updateContactType;
        private readonly PatchContactTypeStatus _patchContactTypeStatus;
        private readonly GetSelectContactType _getSelectContactType;

        public ContactTypeController(
            CreateContactType createContactType,
            GetAllContactType getAllContactType,
            GetContactTypeById getContactTypeById,
            UpdateContactType updateContactType,
            PatchContactTypeStatus patchContactTypeStatus,
            GetSelectContactType getSelectContactType)
        {
            _createContactType = createContactType;
            _getAllContactType = getAllContactType;
            _getContactTypeById = getContactTypeById;
            _updateContactType = updateContactType;
            _patchContactTypeStatus = patchContactTypeStatus;
            _getSelectContactType = getSelectContactType;
        }

        [HttpPost]
        [Route("ContactTypeCreate")]
        public async Task<IActionResult> Create([FromBody] ContactTypeCreateDto dto)
        {
            var result = await _createContactType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ContactTypeList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllContactType.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de contacto." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ContactTypeSelect")]
        public async Task<IActionResult> ContactTypeSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectContactType.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ContactTypeIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long contactTypeId)
        {
            var result = await _getContactTypeById.ExecuteAsync(contactTypeId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de contacto." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ContactTypeUpdate")]
        public async Task<IActionResult> Update([FromBody] ContactTypeUpdateDto dto)
        {
            var result = await _updateContactType.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ContactTypeStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ContactTypeStatusToggleDto dto)
        {
            var result = await _patchContactTypeStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
