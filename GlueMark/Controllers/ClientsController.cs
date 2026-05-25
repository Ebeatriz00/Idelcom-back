using Application.DTOs.Clients;
using Application.UseCases.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly CreateClients _createClients;
        private readonly GetAllClients _getAllClients;
        private readonly GetAllHistoryClients _getAllHistoryClients;
        private readonly GetByIdClients _getClientsById;
        private readonly UpdateClients _updateClients;
        private readonly ExistsContacts _existsContacts;

        private readonly UpdateChangeSalesClients _updateChangeSalesClients;
        private readonly PatchClients _patchClientsStatus;
        private readonly GetSelectClients _getSelectClients;
        private readonly GetClientsDetail _getClientsDetail;

        public ClientsController(
            CreateClients createClients,
            GetAllClients getAllClients,
            GetByIdClients getClientsById,
            UpdateClients updateClients,
            PatchClients patchClientsStatus,
            GetSelectClients getSelectClients,
            UpdateChangeSalesClients updateChangeSalesClients,
            GetAllHistoryClients getAllHistoryClients,
            ExistsContacts existsContacts,
            GetClientsDetail getClientsDetail)
        {
            _createClients = createClients;
            _getAllClients = getAllClients;
            _getClientsById = getClientsById;
            _updateClients = updateClients;
            _patchClientsStatus = patchClientsStatus;
            _getSelectClients = getSelectClients;
            _updateChangeSalesClients = updateChangeSalesClients;
            _getAllHistoryClients = getAllHistoryClients;
            _existsContacts = existsContacts;
            _getClientsDetail = getClientsDetail;
        }

        [HttpGet]
        [Route("ExistsContacts")]
        public async Task<IActionResult> ExistsContacts([FromQuery] long clientsId)
        {
            var result = await _existsContacts.ExecuteAsync(clientsId);
            return Ok(result);
        }


        [HttpPost]
        [Route("ClientsCreate")]
        public async Task<IActionResult> Create([FromBody] ClientsCreateDto dto)
        {
            var result = await _createClients.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ClientsList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null, bool? includeOthers = false)
        {
            var result = await _getAllClients.ExecuteAsync(businessId, search, page, pageSize, usersBy, includeOthers);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de contacto." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }
        [HttpGet]
        [Route("ClientsListHistory")]
        public async Task<IActionResult> GetListHistory([FromQuery] long? clientsId = null)
        {
            var items = await _getAllHistoryClients.ExecuteAsync(clientsId);

            if (items == null || !items.Any())
                return NotFound(new { message = "No se encontró historial para el cliente." });
            return Ok(items);
        }

        [HttpGet]
        [Route("ClientsSelect")]
        public async Task<IActionResult> ClientsSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] long? usersBy = null)
        {
            var result = await _getSelectClients.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            return Ok(result);
        }

        [HttpGet]
        [Route("ClientsIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long clientsId)
        {
            var result = await _getClientsById.ExecuteAsync(clientsId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de contacto." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ClientsUpdate")]
        public async Task<IActionResult> Update([FromBody] ClientsUpdateDto dto)
        {
            var result = await _updateClients.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpPut]
        [Route("ClientsUpdateChangeSales")]
        public async Task<IActionResult> UpdateeChangeSales([FromBody] ClientsUpdateChangeSalesDto dto)
        {
            var result = await _updateChangeSalesClients.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ClientsStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ClientsStatusToggleDto dto)
        {
            var result = await _patchClientsStatus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ClientsDetail")]
        public async Task<IActionResult> GetDetail([FromQuery] long clientsId, [FromQuery] long businessId)
        {
            var result = await _getClientsDetail.ExecuteAsync(clientsId, businessId);

            if (result == null)
                return NotFound(new { message = "Cliente no encontrado o sin datos." });

            return Ok(result);
        }
    }
}
