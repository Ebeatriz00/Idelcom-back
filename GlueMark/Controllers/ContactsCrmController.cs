using Application.DTOs.ContactsCrm;
using Application.UseCases.ConctactsCrm;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsCrmController : ControllerBase
    {

        private readonly CreateContactsCrm _createContactsCrm;
        private readonly GetAllContactsCrm _getAllContactsCrm;
        private readonly GetContactsCrmById _getContactsCrmById;
        private readonly UpdateContactsCrm _updateContactsCrm;
        private readonly PatchContactsCrmStatus _patchContactsCrmStatus;
        private readonly GetSelectContactsCrm _getSelectContactsCrm;

        public ContactsCrmController(
            CreateContactsCrm createContactsCrm,
            GetAllContactsCrm getAllContactsCrm,
            GetContactsCrmById getContactsCrmById,
            UpdateContactsCrm updateContactsCrm,
            PatchContactsCrmStatus patchContactsCrmStatus,
            GetSelectContactsCrm getSelectContactsCrm)
        {
            _createContactsCrm = createContactsCrm;
            _getAllContactsCrm = getAllContactsCrm;
            _getContactsCrmById = getContactsCrmById;
            _updateContactsCrm = updateContactsCrm;
            _patchContactsCrmStatus = patchContactsCrmStatus;
            _getSelectContactsCrm = getSelectContactsCrm;
        }

        [HttpPost]
        [Route("ContactsCrmCreate")] 
        public async Task<IActionResult> Create([FromBody] ContactsCrmCreateDto dto) 
        {
            var result = await _createContactsCrm.ExecuteAsync(dto); 
            return Ok(result);
        }

        [HttpGet]
        [Route("ContactsCrmList")] 
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllContactsCrm.ExecuteAsync(business_id, search, page, pageSize, usersBy); 

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron contactos." }); 

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ContactsCrmSelect")] 
        public async Task<IActionResult> ContactsCrmSelect( 
            [FromQuery] long business_id,
            [FromQuery] long clientsId,
            [FromQuery] long workerId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectContactsCrm.ExecuteAsync(business_id, clientsId, workerId, search, page, pageSize); 
            return Ok(result);
        }

        [HttpGet]
        [Route("ContactsCrmById")] 
        public async Task<IActionResult> GetById([FromQuery] long contactsCrmId) 
        {
            var result = await _getContactsCrmById.ExecuteAsync(contactsCrmId); 

            if (result == null)
                return NotFound(new { message = "No se encontro el contacto." }); 

            return Ok(result);
        }

        [HttpPut]
        [Route("ContactsCrmUpdate")] 
        public async Task<IActionResult> Update([FromBody] ContactsCrmUpdateDto dto) 
        {
            var result = await _updateContactsCrm.ExecuteAsync(dto); 
            return Ok(result);
        }

        [HttpPatch]
        [Route("ContactsCrmStatus")] 
        public async Task<IActionResult> PatchStatus([FromBody] ContactsCrmStatusToggleDto dto) // DTO adaptado
        {
            var result = await _patchContactsCrmStatus.ExecuteAsync(dto); 
            return Ok(result);
        }
    }
}
