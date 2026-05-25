using Application.DTOs.StatePreSale;
using Application.UseCases.LicStatus;
using Application.UseCases.StatePreSale;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class LicStatusController : ControllerBase
        {
            private readonly GetSelectLicStatus _getSelectLicStatus;


            public LicStatusController(
                GetSelectLicStatus getSelectLicStatus)
            {
                _getSelectLicStatus = getSelectLicStatus;

            }

            

            [HttpGet]
            [Route("LicStatusSelect")]
            public async Task<IActionResult> GetSelect(
                [FromQuery] long businessId,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectLicStatus.ExecuteAsync(businessId, search, page, pageSize);
                return Ok(result);
            }
        }
    }
}

