using Application.DTOs.ApiPeru;
using Application.UseCases.ApiPeru;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiPeruController : BaseController
    {
        private readonly ConsultApiPeruRuc _consultApiPeruRuc;

        public ApiPeruController(ConsultApiPeruRuc consultApiPeruRuc)
        {
            _consultApiPeruRuc = consultApiPeruRuc;
        }

        [HttpPost]
        public async Task<IActionResult> ConsultRuc([FromBody] ApiPeruRucLookupRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await _consultApiPeruRuc.ExecuteAsync(dto, cancellationToken);
            return Ok(result);
        }
    }
}
