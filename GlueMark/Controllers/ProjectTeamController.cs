using Application.DTOs.ProjectTeam;
using Application.UseCases.PreSaleProyects;
using Application.UseCases.ProjectTeam;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTeamController : ControllerBase
    {
        private readonly CreateProjectTeam _createProjectTeam;
        private readonly GetAllProjectTeam _getAllProjectTeam;
        private readonly DeleteProjectTeam _deleteProjectTeam;

        public ProjectTeamController(
            CreateProjectTeam createProjectTeam, 
            GetAllProjectTeam getAllProjectTeam,
            DeleteProjectTeam deleteProjectTeam)
        {
            _createProjectTeam = createProjectTeam;
            _getAllProjectTeam = getAllProjectTeam;
            _deleteProjectTeam = deleteProjectTeam;
        }


        [HttpPost]
        [Route("ProjectTeamCreate")]
        public async Task<IActionResult> Create([FromBody] ProjectTeamCreateDto dto)
        {
            var result = await _createProjectTeam.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("PreSaleProyectsList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? projectToken = null, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _getAllProjectTeam.ExecuteAsync(business_id, projectToken, search, page, pageSize);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron trabajo." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }
        [HttpDelete]
        [Route("ProjectTeamDelete")]
        public async Task<IActionResult> Delete([FromBody] ProjectTeamDeleteDto dto)
        {
            var result = await _deleteProjectTeam.ExecuteAsync(dto);
            return Ok(result);
        }


    }
}
