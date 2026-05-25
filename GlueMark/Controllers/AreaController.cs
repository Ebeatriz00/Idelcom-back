using Application.DTOs.Area;
using Application.UseCases.Area;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly CreateArea _createArea;
        private readonly GetAllArea _getAllArea;
        private readonly GetByIdArea _getByIdArea;
        private readonly UpdateArea _updateArea;
        private readonly PatchAreaStatus _patchAreaStatus;
        private readonly GetSelectArea _getSelectArea;

        public AreaController(
            CreateArea createArea,
            GetAllArea getAllArea,
            GetByIdArea getByIdArea,
            UpdateArea updateArea,
            PatchAreaStatus patchAreaStatus,
            GetSelectArea getSelectArea)
        {
            _createArea = createArea;
            _getAllArea = getAllArea;
            _getByIdArea = getByIdArea;
            _updateArea = updateArea;
            _patchAreaStatus = patchAreaStatus;
            _getSelectArea = getSelectArea;
        }

        [HttpPost]
        [Route("AreaCreate")]
        public async Task<IActionResult> Create([FromBody] AreaCreateDto dto)
        {
            var result = await _createArea.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("AreaList")]
        public async Task<IActionResult> GetList([FromQuery] int business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllArea.ExecuteAsync(business_id,search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron áreas." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }

        [HttpGet]
        [Route("AreaSelect")]
        public async Task<IActionResult> AreaSelect(
        [FromQuery] long business_id,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectArea.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }



        [HttpGet]
        [Route("AreaIdList")]
        public async Task<IActionResult> GetListId([FromQuery] int areaId)
        {
            var result = await _getByIdArea.ExecuteAsync(areaId);
            if (result == null)
                return NotFound(new { message = "No se encontró el área." });

            return Ok(result);
        }

        [HttpPut]
        [Route("AreaUpdate")]
        public async Task<IActionResult> Update([FromBody] AreaUpdateDto dto)
        {
            var result = await _updateArea.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("AreaStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] AreaStatusToggleDto dto)
        {
            var result = await _patchAreaStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}

