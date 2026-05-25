using Application.DTOs.Suppliers;
using Application.UseCases.Brands;
using Application.UseCases.Suppliers;
using Azure;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : BaseController
    {
        private readonly CreateSuppliers _createSuppliers;
        private readonly GetAllSuppliers _getAllSuppliers;
        private readonly GetSuppliersById _getSuppliersById;
        private readonly UpdateSuppliers _updateSuppliers;
        private readonly PatchSuppliersStatus _patchSuppliersStatus;
        private readonly GetSelectSuppliers _getSelectSuppliers;

        public SuppliersController(
            CreateSuppliers createSuppliers,
            GetAllSuppliers getAllSuppliers,
            GetSuppliersById getSuppliersById,
            UpdateSuppliers updateSuppliers,
            PatchSuppliersStatus patchSuppliersStatus,
            GetSelectSuppliers getSelectSuppliers)
        {
            _createSuppliers = createSuppliers;
            _getAllSuppliers = getAllSuppliers;
            _getSuppliersById = getSuppliersById;
            _updateSuppliers = updateSuppliers;
            _patchSuppliersStatus = patchSuppliersStatus;
            _getSelectSuppliers = getSelectSuppliers;
        }

        [HttpPost]
        [Route("SuppliersCreate")]
        public async Task<IActionResult> Create([FromBody] SuppliersCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createSuppliers.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("SuppliersList")]
        public async Task<IActionResult> GetList([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();

            var result = await _getAllSuppliers.ExecuteAsync(businessId, search, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron proveedores." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("SuppliersSelect")]
        public async Task<IActionResult> SuppliersSelect(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectSuppliers.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("SuppliersById")]
        public async Task<IActionResult> GetById([FromQuery] long suppliersId)
        {
            var result = await _getSuppliersById.ExecuteAsync(suppliersId);

            if (result == null)
                return NotFound(new { message = "No se encontro el proveedor." });

            return Ok(result);
        }

        [HttpPut]
        [Route("SuppliersUpdate")]
        public async Task<IActionResult> Update([FromBody] SuppliersUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _updateSuppliers.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("SuppliersStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] SuppliersStatusToggleDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _patchSuppliersStatus.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}