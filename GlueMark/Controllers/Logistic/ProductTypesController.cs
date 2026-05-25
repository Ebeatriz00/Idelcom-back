using Application.DTOs.ProductTypes;
using Application.UseCases.ProductTypes;
using Azure;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : BaseController
    {
        private readonly CreateProductTypes _createProductTypes;
        private readonly GetAllProductTypes _getAllProductTypes;
        private readonly GetProductTypesById _getProductTypesById;
        private readonly UpdateProductTypes _updateProductTypes;
        private readonly PatchProductTypesStatus _patchProductTypesStatus;
        private readonly GetSelectProductTypes _getSelectProductTypes;

        public ProductTypesController(
            CreateProductTypes createProductTypes,
            GetAllProductTypes getAllProductTypes,
            GetProductTypesById getProductTypesById,
            UpdateProductTypes updateProductTypes,
            PatchProductTypesStatus patchProductTypesStatus,
            GetSelectProductTypes getSelectProductTypes)
        {
            _createProductTypes = createProductTypes;
            _getAllProductTypes = getAllProductTypes;
            _getProductTypesById = getProductTypesById;
            _updateProductTypes = updateProductTypes;
            _patchProductTypesStatus = patchProductTypesStatus;
            _getSelectProductTypes = getSelectProductTypes;
        }

        [HttpPost]
        [Route("ProductTypesCreate")]
        public async Task<IActionResult> Create([FromBody] ProductTypesCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createProductTypes.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductTypesList")]
        public async Task<IActionResult> GetList( [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllProductTypes.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron tipos de producto." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ProductTypesSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectProductTypes.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductTypesById")]
        public async Task<IActionResult> GetListId([FromQuery] long productTypesId)
        {
            var result = await _getProductTypesById.ExecuteAsync(productTypesId);
            if (result == null)
                return NotFound(new { message = "No se encontró el tipo de producto." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ProductTypesUpdate")]
        public async Task<IActionResult> Update([FromBody] ProductTypesUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateProductTypes.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ProductTypesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ProductTypesStatusToggleDto dto)
        {
            var result = await _patchProductTypesStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
