using Application.DTOs.ClientsActivity;
using Application.UseCases.ClientsActivity;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsActivityController : ControllerBase
    {
        private readonly CreateClientsActivity _createClientsActivity;
        private readonly GetClientsActivities _getClientsActivities;
        private readonly DeleteClientsActivity _deleteClientsActivity;
        private readonly UpdateActivityStatus _updateActivityStatus;

        public ClientsActivityController(
            CreateClientsActivity createClientsActivity,
            GetClientsActivities getClientsActivities,
            DeleteClientsActivity deleteClientsActivity,
            UpdateActivityStatus updateActivityStatus)
        {
            _createClientsActivity = createClientsActivity;
            _getClientsActivities = getClientsActivities;
            _deleteClientsActivity = deleteClientsActivity;
            _updateActivityStatus = updateActivityStatus;
        }

        [HttpPost]
        [Route("ActivityCreate")]
        public async Task<IActionResult> Create([FromBody] ClientActivityCreateDto dto)
        {
            var result = await _createClientsActivity.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("ActivityList")]
        public async Task<IActionResult> GetList(
        [FromQuery] long business_id,
        [FromQuery] long clients_id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            var result = await _getClientsActivities.ExecuteAsync(business_id, clients_id, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron actividades." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpPatch]
        [Route("ActivityStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] ClientActivityUpdateDto dto)
        {
            var result = await _updateActivityStatus.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        [Route("ActivityDelete")]
        public async Task<IActionResult> Delete([FromBody] ClientsActivityDeleteDto dto)
        {
            var result = await _deleteClientsActivity.ExecuteAsync(dto);
            return Ok(result);
        }



    }
}
