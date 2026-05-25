using Application.UseCases.SupplierGroups;
using Application.UseCases.TypeSuppliers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeSuppliersController : ControllerBase
    {
        private readonly GetSelectTypeSuppliers _getSelectTypeSuppliers;

        public TypeSuppliersController(GetSelectTypeSuppliers getSelectTypeSuppliers)
        {
            _getSelectTypeSuppliers = getSelectTypeSuppliers;
        }
        [HttpGet]
        [Route("TypeSupplierSelect")]
        public async Task<IActionResult> GetSelect(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectTypeSuppliers.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

    }
}
