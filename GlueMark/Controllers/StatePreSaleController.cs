using Application.DTOs.StatePreSale;
using Application.UseCases.StatePreSale;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    namespace Idelcom.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class StatePreSaleController : ControllerBase
        {
            private readonly CreateStatePreSale _createStatePreSale;
            private readonly GetAllStatePreSale _getAllStatePreSale;
            private readonly GetStatePreSaleById _getStatePreSaleById; 
            private readonly UpdateStatePreSale _updateStatePreSale;
            private readonly PatchStatePreSaleStatus _patchStatePreSaleStatus; 
            private readonly GetSelectStatePreSale _getSelectStatePreSale; 

            public StatePreSaleController(
                CreateStatePreSale createStatePreSale,
                GetAllStatePreSale getAllStatePreSale,
                GetStatePreSaleById getStatePreSaleById,
                UpdateStatePreSale updateStatePreSale,
                PatchStatePreSaleStatus patchStatePreSaleStatus,
                GetSelectStatePreSale getSelectStatePreSale)
            {
                _createStatePreSale = createStatePreSale;
                _getAllStatePreSale = getAllStatePreSale;
                _getStatePreSaleById = getStatePreSaleById;
                _updateStatePreSale = updateStatePreSale;
                _patchStatePreSaleStatus = patchStatePreSaleStatus;
                _getSelectStatePreSale = getSelectStatePreSale;
            }

            [HttpPost]
            [Route("StatePreSaleCreate")] 
            public async Task<IActionResult> Create([FromBody] StatePreSaleCreateDto dto)
            {
                var result = await _createStatePreSale.ExecuteAsync(dto);
                return Ok(result);
            }

            [HttpGet]
            [Route("StatePreSaleList")]
            public async Task<IActionResult> GetList([FromQuery] long businessId, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                var result = await _getAllStatePreSale.ExecuteAsync(businessId, search, page, pageSize);

                if (result == null || !result.Items.Any())
                    return NotFound(new { message = "No se encontraron estados de preventa." });

                Response.Headers["X-Total-Count"] = result.Total.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

                return Ok(result.Items);
            }

            [HttpGet]
            [Route("StatePreSaleSelect")]
            public async Task<IActionResult> GetSelect(
                [FromQuery] long businessId,
                [FromQuery] string? search = null,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 20)
            {
                var result = await _getSelectStatePreSale.ExecuteAsync(businessId, search, page, pageSize);
                return Ok(result);
            }

            [HttpGet]
            [Route("StatePreSaleById")]
            public async Task<IActionResult> GetById([FromQuery] long statePreSaleId) // Tipo ajustado a long
            {
                var result = await _getStatePreSaleById.ExecuteAsync(statePreSaleId);
                if (result == null)
                    return NotFound(new { message = "Estado de preventa no encontrado." });

                return Ok(result);
            }

            [HttpPut]
            [Route("StatePreSaleUpdate")]
            public async Task<IActionResult> Update([FromBody] StatePreSaleUpdateDto dto)
            {
                var result = await _updateStatePreSale.ExecuteAsync(dto);
                return Ok(result);
            }
            [HttpPatch]
            [Route("StatePreSaleStatus")]
            public async Task<IActionResult> PatchStatus([FromBody] StatePreSaleStatusToggleDto dto)
            {
                var result = await _patchStatePreSaleStatus.ExecuteAsync(dto);
                return Ok(result);
            }
        }
    }
}
