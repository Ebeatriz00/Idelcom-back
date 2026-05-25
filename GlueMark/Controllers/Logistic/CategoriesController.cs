using Application.DTOs.Categories;
using Application.UseCases.Categories;
using Core.Entities;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Idelcom.Controllers.Logistic
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseController
    {
        private readonly CreateCategories _createCategories;
        private readonly GetAllCategories _getAllCategories;
        private readonly GetCategoriesById _getCategoriesById;
        private readonly UpdateCategories _updateCategories;
        private readonly PatchCategoriesStatus _patchCategoriesStatus;
        private readonly GetSelectCategories _getSelectCategories;

        public CategoriesController(
            CreateCategories createCategories,
            GetAllCategories getAllCategories,
            GetCategoriesById getCategoriesById,
            UpdateCategories updateCategories,
            PatchCategoriesStatus patchCategoriesStatus,
            GetSelectCategories getSelectCategories)
        {
            _createCategories = createCategories;
            _getAllCategories = getAllCategories;
            _getCategoriesById = getCategoriesById;
            _updateCategories = updateCategories;
            _patchCategoriesStatus = patchCategoriesStatus;
            _getSelectCategories = getSelectCategories;
        }

        [HttpPost]
        [Route("CategoriesCreate")]
        public async Task<IActionResult> Create([FromBody] CategoriesCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _createCategories.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpGet]
        [Route("CategoriesList")]
        public async Task<IActionResult> GetList([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllCategories.ExecuteAsync(businessId, search, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron categorías." });

            return Ok(result);
        }

        [HttpGet]
        [Route("CategoriesSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectCategories.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("CategoriesById")]
        public async Task<IActionResult> GetListId([FromQuery] long categoriesId)
        {
            var result = await _getCategoriesById.ExecuteAsync(categoriesId);
            if (result == null)
                return NotFound(new { message = "No se encontró la categoría." });

            return Ok(result);
        }

        [HttpPut]
        [Route("CategoriesUpdate")]
        public async Task<IActionResult> Update([FromBody] CategoriesUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _updateCategories.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }

        [HttpPatch]
        [Route("CategoriesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] CategoriesStatusToggleDto dto)
        {
            var userId = GetCurrentUserId();
            var businessId = GetCurrentBusinessId();
            var result = await _patchCategoriesStatus.ExecuteAsync(dto, userId, businessId);
            return Ok(result);
        }
    }
}
