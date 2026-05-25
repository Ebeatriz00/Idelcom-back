using Application.DTOs.JobTitle;
using Application.UseCases.Currency;
using Application.UseCases.JobTitle;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTitleController : ControllerBase
    {
        private readonly CreateJobTitle _createJobTitle;
        private readonly GetAllJobTitle _getAllJobTitle;
        private readonly GetJobTitleById _getJobTitleById;
        private readonly UpdateJobTitle _updateJobTitle;
        private readonly PatchJobTitleStatus _patchJobTitleStatus;
        private readonly GetSelectJobTitle _getSelectJobTitle;

        public JobTitleController(
            CreateJobTitle createJobTitle,
            GetAllJobTitle getAllJobTitle,
            GetJobTitleById getJobTitleById,
            UpdateJobTitle updateJobTitle,
            PatchJobTitleStatus patchJobTitleStatus,
            GetSelectJobTitle getSelectJobTitle)
        {
            _createJobTitle = createJobTitle;
            _getAllJobTitle = getAllJobTitle;
            _getJobTitleById = getJobTitleById;
            _updateJobTitle = updateJobTitle;
            _patchJobTitleStatus = patchJobTitleStatus;
            _getSelectJobTitle = getSelectJobTitle;
        }

        [HttpPost]
        [Route("JobTitleCreate")]
        public async Task<IActionResult> Create([FromBody] JobTitleCreateDto dto)
        {
            var result = await _createJobTitle.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("JobTitleList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllJobTitle.ExecuteAsync(business_id,search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron cargos." });

            return Ok(result);
        }

        [HttpGet]
        [Route("JobTitleSelect")]
        public async Task<IActionResult> JobTitleSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectJobTitle.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("JobTitleIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long jobTitleId)
        {
            var result = await _getJobTitleById.ExecuteAsync(jobTitleId);
            if (result == null)
                return NotFound(new { message = "No se encontró el cargo." });

            return Ok(result);
        }

        [HttpPut]
        [Route("JobTitleUpdate")]
        public async Task<IActionResult> Update([FromBody] JobTitleUpdateDto dto)
        {
            var result = await _updateJobTitle.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("JobTitleStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] JobTitleStatusToggleDto dto)
        {
            var result = await _patchJobTitleStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
