using Application.DTOs.Brands;
using Application.UseCases.Brands;
using Azure;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : BaseController
    {
        private readonly CreateBrands _createBrands;
        private readonly GetAllBrands _getAllBrands;
        private readonly GetBrandsById _getBrandsById;
        private readonly UpdateBrands _updateBrands;
        private readonly PatchBrandsStatus _patchBrandsStatus;
        private readonly GetSelectBrands _getSelectBrands;

        public BrandsController(
            CreateBrands createBrands,
            GetAllBrands getAllBrands,
            GetBrandsById getBrandsById,
            UpdateBrands updateBrands,
            PatchBrandsStatus patchBrandsStatus,
            GetSelectBrands getSelectBrands)
        {
            _createBrands = createBrands;
            _getAllBrands = getAllBrands;
            _getBrandsById = getBrandsById;
            _updateBrands = updateBrands;
            _patchBrandsStatus = patchBrandsStatus;
            _getSelectBrands = getSelectBrands;
        }

        [HttpPost]
        [Route("BrandsCreate")]
        public async Task<IActionResult> Create([FromBody] BrandsCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createBrands.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("BrandsList")]
        public async Task<IActionResult> GetList([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllBrands.ExecuteAsync(businessId, search, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron marcas." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("BrandsSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectBrands.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("BrandsById")]
        public async Task<IActionResult> GetListId([FromQuery] long brandsId)
        {
            var result = await _getBrandsById.ExecuteAsync(brandsId);
            if (result == null)
                return NotFound(new { message = "No se encontró la marca." });

            return Ok(result);
        }

        [HttpPut]
        [Route("BrandsUpdate")]
        public async Task<IActionResult> Update([FromBody] BrandsUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateBrands.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("BrandsStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] BrandsStatusToggleDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _patchBrandsStatus.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}
