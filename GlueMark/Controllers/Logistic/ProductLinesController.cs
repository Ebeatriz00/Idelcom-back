using Application.DTOs.ProductLines;
using Application.UseCases.ProductLines;
using Azure;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductLinesController : BaseController
    {
        private readonly CreateProductLines _createProductLines;
        private readonly GetAllProductLines _getAllProductLines;
        private readonly GetProductLinesById _getProductLinesById;
        private readonly UpdateProductLines _updateProductLines;
        private readonly PatchProductLinesStatus _patchProductLinesStatus;
        private readonly GetSelectProductLines _getSelectProductLines;

        public ProductLinesController(
            CreateProductLines createProductLines,
            GetAllProductLines getAllProductLines,
            GetProductLinesById getProductLinesById,
            UpdateProductLines updateProductLines,
            PatchProductLinesStatus patchProductLinesStatus,
            GetSelectProductLines getSelectProductLines)
        {
            _createProductLines = createProductLines;
            _getAllProductLines = getAllProductLines;
            _getProductLinesById = getProductLinesById;
            _updateProductLines = updateProductLines;
            _patchProductLinesStatus = patchProductLinesStatus;
            _getSelectProductLines = getSelectProductLines;
        }

        [HttpPost]
        [Route("ProductLinesCreate")]
        public async Task<IActionResult> Create([FromBody] ProductLinesCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createProductLines.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductLinesList")]
        public async Task<IActionResult> GetList([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllProductLines.ExecuteAsync(businessId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron líneas de producto." });
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductLinesSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] long? categoriesId = null)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectProductLines.ExecuteAsync(businessId, categoriesId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductLinesById")]
        public async Task<IActionResult> GetListId([FromQuery] long productLinesId)
        {
            var result = await _getProductLinesById.ExecuteAsync(productLinesId);
            if (result == null)
                return NotFound(new { message = "No se encontró la línea de producto." });

            return Ok(result);
        }

        [HttpPut]
        [Route("ProductLinesUpdate")]
        public async Task<IActionResult> Update([FromBody] ProductLinesUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateProductLines.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("ProductLinesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] ProductLinesStatusToggleDto dto)
        {
            var result = await _patchProductLinesStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
