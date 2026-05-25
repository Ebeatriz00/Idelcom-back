using Application.DTOs.Boxes;
using Application.UseCases.Boxes;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoxesController : ControllerBase
    {
        private readonly CreateBoxes _createBoxes;
        private readonly GetAllBoxes _getAllBoxes;
        private readonly GetBoxesById _getBoxesById;
        private readonly UpdateBoxes _updateBoxes;
        private readonly PatchBoxesStatus _patchBoxesStatus;
        private readonly GetSelectBoxes _getSelectBoxes;

        public BoxesController(
            CreateBoxes createBoxes,
            GetAllBoxes getAllBoxes,
            GetBoxesById getBoxesById,
            UpdateBoxes updateBoxes,
            PatchBoxesStatus patchBoxesStatus,
            GetSelectBoxes getSelectBoxes)
        {
            _createBoxes = createBoxes;
            _getAllBoxes = getAllBoxes;
            _getBoxesById = getBoxesById;
            _updateBoxes = updateBoxes;
            _patchBoxesStatus = patchBoxesStatus;
            _getSelectBoxes = getSelectBoxes;
        }

        [HttpPost]
        [Route("BoxesCreate")]
        public async Task<IActionResult> Create([FromBody] BoxesCreateDto dto)
        {
            var result = await _createBoxes.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("BoxesList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllBoxes.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron cajas." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("BoxesSelect")]
        public async Task<IActionResult> BoxesSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectBoxes.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("BoxesById")]
        public async Task<IActionResult> GetById([FromQuery] long boxesId)
        {
            var result = await _getBoxesById.ExecuteAsync(boxesId);
            if (result == null)
                return NotFound(new { message = "No se encontro la caja." });
            return Ok(result);
        }

        [HttpPut]
        [Route("BoxesUpdate")]
        public async Task<IActionResult> Update([FromBody] BoxesUpdateDto dto)
        {
            var result = await _updateBoxes.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("BoxesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] BoxesStatusToggleDto dto)
        {
            var result = await _patchBoxesStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
