using Application.DTOs.Area;
using Application.DTOs.CommercialParameters;
using Application.UseCases.Area;
using Application.UseCases.CommercialParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommercialParametersController : ControllerBase
    {

        private readonly CreateCommercialParameters _createCommercialParameters;
        private readonly GetAllCommercialParameters _getAllCommercialParameters;
        private readonly GetByIdCommercialParameters _getByIdCommercialParameters;
        private readonly UpdateCommercialParameters _updateCommercialParameters;
        private readonly PatchCommercialParameters _patchCommercialParameters;

        public CommercialParametersController(CreateCommercialParameters createCommercialParameters, GetAllCommercialParameters getAllCommercialParameters, GetByIdCommercialParameters getByIdCommercialParameters, UpdateCommercialParameters updateCommercialParameters, PatchCommercialParameters patchCommercialParameters)
        {
            _createCommercialParameters = createCommercialParameters;
            _getAllCommercialParameters = getAllCommercialParameters;
            _getByIdCommercialParameters = getByIdCommercialParameters;
            _updateCommercialParameters = updateCommercialParameters;
            _patchCommercialParameters = patchCommercialParameters;
        }

        [HttpPost]
        [Route("CommercialParametersCreate")]
        public async Task<IActionResult> Create([FromBody] CreateCommercialParametersDto dto)
        {
            var result = await _createCommercialParameters.ExecuteAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        [Route("CommercialParametersList")]
        public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllCommercialParameters.ExecuteAsync(businessId, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron parámetros comerciales." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            return Ok(result);
        }

        [HttpGet]
        [Route("CommercialParametersById")]
        public async Task<IActionResult> GetListId([FromQuery] int commercialParametersId)
        {
            var result = await _getByIdCommercialParameters.ExecuteAsync(commercialParametersId);
            if (result == null)
                return NotFound(new { message = "No se encontró el área." });

            return Ok(result);
        }

        [HttpPut]
        [Route("CommercialParametersUpdate")]
        public async Task<IActionResult> Update([FromBody] UpdateCommercialParametersDto dto)
        {
            var result = await _updateCommercialParameters.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("CommercialParametersStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] CommercialParametersStatusToggleDto dto)
        {
            var result = await _patchCommercialParameters.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
