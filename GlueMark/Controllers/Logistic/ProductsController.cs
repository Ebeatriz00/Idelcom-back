using Application.DTOs.Products;
using Application.UseCases.Products;
using Azure;
using Core.Interfaces.Services;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly CreateProducts _createProducts;
        private readonly GetAllProducts _getAllProducts;
        private readonly GetSelectProducts _getSelectProducts;
        private readonly GetByIdProducts _getByIdProducts;
        private readonly UpdateProducts _updateProducts;
        private readonly PatchProducts _patchProducts;

        public ProductsController(
            CreateProducts createProducts,
            GetAllProducts getAllProducts,
            GetSelectProducts getSelectProducts,
            GetByIdProducts getByIdProducts,
            UpdateProducts updateProducts,
            PatchProducts patchProducts)
        {
            _createProducts = createProducts;
            _getAllProducts = getAllProducts;
            _getSelectProducts = getSelectProducts;
            _getByIdProducts = getByIdProducts;
            _updateProducts = updateProducts;
            _patchProducts = patchProducts;
        }

        [HttpPost]
        [Route("ProductsCreate")]
        public async Task<IActionResult> Create([FromBody] ProductsCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createProducts.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductsList")]
        public async Task<IActionResult> GetList([FromQuery] long? categoriesId = null, [FromQuery] long? productTypeId = null, [FromQuery] long? brandsId = null, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllProducts.ExecuteAsync(businessId, categoriesId, productTypeId, brandsId, search, page, pageSize);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron productos." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("ProductsSelect")]
        public async Task<IActionResult> ProductsSelect(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectProducts.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductsById")]
        public async Task<IActionResult> GetById([FromQuery] long productsId)
        {
           
            var result = await _getByIdProducts.ExecuteAsync(productsId);
            if (result == null)
                return NotFound(new { message = "No se encontró el producto." });
            return Ok(result);
        }

        [HttpPut]
        [Route("ProductsUpdate")]
        public async Task<IActionResult> Update([FromBody] ProductsUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateProducts.ExecuteAsync(dto, userId, businessId);
            if (result == null)
                return NotFound(new { message = "No se encontró el producto para actualizar." });
            return Ok(result);
        }

        [HttpPatch]
        [Route("ProductsStatus")]
        public async Task<IActionResult> Patch([FromBody] ProductsStatusToggleDto dto)
        {

            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _patchProducts.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}
