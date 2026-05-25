using Application.UseCases.AuxiliarType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuxiliarTypeController : ControllerBase
    {
        private readonly GetSelectAuxiliarType _getSelectAuxiliarType;
        public AuxiliarTypeController(GetSelectAuxiliarType getSelectAuxiliarType)
        {
            _getSelectAuxiliarType = getSelectAuxiliarType;
        }
        [HttpGet]
        [Route("AuxiliarTypeSelect")]
        public async Task<IActionResult> GetAuxiliarTypeSelect([FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectAuxiliarType.ExecuteAsync(search, page, pageSize);
            return Ok(result);
        }
    }
}
