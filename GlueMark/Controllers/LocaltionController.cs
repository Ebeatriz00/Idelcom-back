using Application.UseCases.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocaltionController : ControllerBase
    {
        private readonly GetSelectDepartment _getSelectDepartment;
        private readonly GetSelectProvince _getSelectProvince;
        private readonly GetSelectDistrict _getSelectDistrict;

        public LocaltionController(GetSelectDepartment getSelectDepartment, GetSelectProvince getSelectProvince, GetSelectDistrict getSelectDistrict)
        {
            _getSelectDepartment = getSelectDepartment;
            _getSelectProvince = getSelectProvince;
            _getSelectDistrict = getSelectDistrict;
        }
        [HttpGet]
        [Route("DepartmentSelect")]
        public async Task<IActionResult> GetDepartmentSelect(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectDepartment.ExecuteAsync(search, page, pageSize);
            return Ok(result);
        }
        [HttpGet]
        [Route("ProvinceSelect")]
        public async Task<IActionResult> GetProvinceSelect(
         [FromQuery] int departmentId,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
            )
        {
            var result = await _getSelectProvince.ExecuteAsync(departmentId, search, page, pageSize);
            return Ok(result);

        }

        [HttpGet]
        [Route("DistrictSelect")]
        public async Task<IActionResult> GetDistrictSelect(
        [FromQuery] int departmentId,
        [FromQuery] int provinceId,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
            )
        {
            var result = await _getSelectDistrict.ExecuteAsync(departmentId, provinceId, search, page, pageSize);
            return Ok(result);

        }
    }
}
