using Application.UseCases.TypeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeAnalysisController : ControllerBase
    {
        private readonly GetSelectTypeAnalysis _getSelectTypeAnalysis;
        public TypeAnalysisController(GetSelectTypeAnalysis getSelectTypeAnalysis)
        {
            _getSelectTypeAnalysis = getSelectTypeAnalysis;
        }
        [HttpGet]
        [Route("TypeAnalysisSelect")]
        public async Task<IActionResult> GetTypeAnalysisSelect([FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectTypeAnalysis.ExecuteAsync(search, page, pageSize);
            return Ok(result);
        }
    }
}
