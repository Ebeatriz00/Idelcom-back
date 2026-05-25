using Application.DTOs.CostCenters;
using Application.UseCases.CostCenters;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CostCentersController : ControllerBase
    {
        private readonly CreateCostCenters _createCostCenters;
        private readonly GetAllCostCenters _getAllCostCenters;
        private readonly GetByIdCostCenters _getByIdCostCenters;
        private readonly UpdateCostCenters _updateCostCenters;
        private readonly PatchCostCentersStatus _patchCostCentersStatus;
        private readonly GetSelectCostCenters _getSelectCostCenters;

        public CostCentersController(
            CreateCostCenters createCostCenters,
            GetAllCostCenters getAllCostCenters,
            GetByIdCostCenters getByIdCostCenters,
            UpdateCostCenters updateCostCenters,
            PatchCostCentersStatus patchCostCentersStatus,
            GetSelectCostCenters getSelectCostCenters)
        {
            _createCostCenters = createCostCenters;
            _getAllCostCenters = getAllCostCenters;
            _getByIdCostCenters = getByIdCostCenters;
            _updateCostCenters = updateCostCenters;
            _patchCostCentersStatus = patchCostCentersStatus;
            _getSelectCostCenters = getSelectCostCenters;
        }

        [HttpPost]
        [Route("CostCentersCreate")]
        public async Task<IActionResult> Create([FromBody] CostCentersCreateDto dto)
        {
            var result = await _createCostCenters.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("CostCentersList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllCostCenters.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron centros de costo." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("CostCentersSelect")]
        public async Task<IActionResult> GetSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectCostCenters.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("CostCentersById")]
        public async Task<IActionResult> GetById([FromQuery] long costCentersId)
        {
            var result = await _getByIdCostCenters.ExecuteAsync(costCentersId);
            if (result == null)
                return NotFound(new { message = "No se encontró el centro de costo." });

            return Ok(result);
        }

        [HttpPut]
        [Route("CostCentersUpdate")]
        public async Task<IActionResult> Update([FromBody] CostCentersUpdateDto dto)
        {
            var result = await _updateCostCenters.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("CostCentersStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] CostCentersStatusToggleDto dto)
        {
            var result = await _patchCostCentersStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
