using Application.DTOs.FileTrackingProducts;
using Application.UseCases.FileTrackingProducts;
using Azure;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductFilesController : BaseController
    {
        private readonly CreateProductFile _createProductFile;
        private readonly GetAllProductFiles _getAllProductFiles;
        private readonly DeleteProductFile _deleteProductFile;

        public ProductFilesController(
            CreateProductFile createProductFile,
            GetAllProductFiles getAllProductFiles,
            DeleteProductFile deleteProductFile)
        {
            _createProductFile = createProductFile;
            _getAllProductFiles = getAllProductFiles;
            _deleteProductFile = deleteProductFile;
        }

        [HttpPost]
        [Route("ProductFileCreate")]
        public async Task<IActionResult> Create(
            [FromBody] FileTrackingProductsCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();

            var result = await _createProductFile.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("ProductFilesList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long productsId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllProductFiles.ExecuteAsync(productsId, businessId, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron archivos para este producto." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpDelete]
        [Route("ProductFileDelete")]
        public async Task<IActionResult> Delete(
            [FromBody] FileTrackingProductsDeleteDto dto
           )
        {
            var businessId = GetCurrentBusinessId();
            var result = await _deleteProductFile.ExecuteAsync(dto, businessId);
            return Ok(result);
        }
    }
}
