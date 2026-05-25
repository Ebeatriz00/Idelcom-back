using Application.DTOs.SupplierGroups;
using Application.UseCases.SupplierGroups;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierGroupsController : ControllerBase
    {
        private readonly CreateSupplierGroups _createSupplierGroups;
        private readonly GetAllSupplierGroups _getAllSupplierGroups;
        private readonly GetByIdSupplierGroups _getByIdSupplierGroups;
        private readonly UpdateSupplierGroups _updateSupplierGroups;
        private readonly PatchSupplierGroupsStatus _patchSupplierGroupsStatus;
        private readonly GetSelectSupplierGroups _getSelectSupplierGroups;

        public SupplierGroupsController(
            CreateSupplierGroups createSupplierGroups,
            GetAllSupplierGroups getAllSupplierGroups,
            GetByIdSupplierGroups getByIdSupplierGroups,
            UpdateSupplierGroups updateSupplierGroups,
            PatchSupplierGroupsStatus patchSupplierGroupsStatus,
            GetSelectSupplierGroups getSelectSupplierGroups)
        {
            _createSupplierGroups = createSupplierGroups;
            _getAllSupplierGroups = getAllSupplierGroups;
            _getByIdSupplierGroups = getByIdSupplierGroups;
            _updateSupplierGroups = updateSupplierGroups;
            _patchSupplierGroupsStatus = patchSupplierGroupsStatus;
            _getSelectSupplierGroups = getSelectSupplierGroups;
        }

        [HttpPost]
        [Route("SupplierGroupsCreate")]
        public async Task<IActionResult> Create([FromBody] SupplierGroupsCreateDto dto)
        {
            var result = await _createSupplierGroups.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("SupplierGroupsList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllSupplierGroups.ExecuteAsync(business_id, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron grupos de proveedores." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("SupplierGroupsSelect")]
        public async Task<IActionResult> GetSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectSupplierGroups.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("SupplierGroupsById")]
        public async Task<IActionResult> GetById([FromQuery] long supplierGroupsId)
        {
            var result = await _getByIdSupplierGroups.ExecuteAsync(supplierGroupsId);
            if (result == null)
                return NotFound(new { message = "No se encontró el grupo de proveedores." });

            return Ok(result);
        }

        [HttpPut]
        [Route("SupplierGroupsUpdate")]
        public async Task<IActionResult> Update([FromBody] SupplierGroupsUpdateDto dto)
        {
            var result = await _updateSupplierGroups.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("SupplierGroupsStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] SupplierGroupsStatusToggleDto dto)
        {
            var result = await _patchSupplierGroupsStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
