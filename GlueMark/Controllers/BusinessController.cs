using Application.DTOs.Business;
using Application.DTOs.Currency;
using Application.UseCases.Business;
using Application.UseCases.Currency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly CreateBusiness _createBusiness;
        private readonly GetViewBusiness _getViewBusiness;

        public BusinessController(CreateBusiness createBusiness, GetViewBusiness getViewBusiness)
        {
            _createBusiness = createBusiness;
            _getViewBusiness = getViewBusiness;
        }
        [HttpPost]
        [Route("BusinessCreate")]
        public async Task<IActionResult> Create([FromBody] BusinessCreateDto dto)
        {
            var result = await _createBusiness.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("BusinessView")]
        //[Authorize]
        public async Task<IActionResult> GetListViewId([FromQuery] long BusinessId)
        {
            var result = await _getViewBusiness.ExecuteAsync(BusinessId);
            if (result == null)
                return NotFound(new { message = "No se encontro la empresa." });

            return Ok(result);
        }
    }
}
