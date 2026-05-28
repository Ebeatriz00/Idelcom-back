using Application.DTOs.Orders;
using Application.UseCases.Orders;
using Application.UseCases.PreSaleProyects.Application.UseCases.PreSaleProyects;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly GetAllOrders _getAllOrders;
        private readonly RegisterSsomaOrder _registerSsomaOrder;
        private readonly RegisterQualitySupervisor _registerQualitySupervisor;
        private readonly CreateProjectManager _createProjectManager;

        public OrdersController(
            
            GetAllOrders getAllOrders,
            RegisterSsomaOrder registerSsomaOrder,
            RegisterQualitySupervisor registerQualitySupervisor,
            CreateProjectManager createProjectManager)

        {
            _getAllOrders = getAllOrders;
            _registerSsomaOrder = registerSsomaOrder;
            _registerQualitySupervisor = registerQualitySupervisor;
            _createProjectManager = createProjectManager;
        }

        [HttpGet]   
        [Route("OrdersList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] long? responsibleStaff = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10 )

        {
            var result = await _getAllOrders.ExecuteAsync( businessId, search, responsibleStaff, page, pageSize );
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron Ordenes" });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpPut]
        [Route("RegisterSsoma")]
        public async Task<IActionResult> RegisterSsoma([FromBody] OrdersSsomaRegister dto)
        {
            var result = await _registerSsomaOrder.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Route("RegisterQualitySupervisor")]
        public async Task<IActionResult> RegisterQualitySupervisor(QualitySupervisorCreateDto dto)
        {
            var result = await _registerQualitySupervisor.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Route("RegisterProjectManager")]
        public async Task<IActionResult> RegisterProjectManager(ProjectManagerCreateDto dto)
        {
            var result = await _createProjectManager.ExecuteAsync(dto);
            return Ok(result);
        }

    }
}
