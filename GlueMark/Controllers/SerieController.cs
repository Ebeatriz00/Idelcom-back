using Application.DTOs.Series;
using Application.UseCases.Series;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly CreateSeries _createSeries;
        private readonly GetAllSeries _getAllSeries;
        private readonly GetSeriesById _getSeriesById;
        private readonly UpdateSeries _updateSeries;
        private readonly PatchSeriesStatus _patchSeriesStatus;
        private readonly GetSelectSeries _getSelectSeries;

        public SeriesController(
            CreateSeries createSeries,
            GetAllSeries getAllSeries,
            GetSeriesById getSeriesById,
            UpdateSeries updateSeries,
            PatchSeriesStatus patchSeriesStatus,
            GetSelectSeries getSelectSeries)
        {
            _createSeries = createSeries;
            _getAllSeries = getAllSeries;
            _getSeriesById = getSeriesById;
            _updateSeries = updateSeries;
            _patchSeriesStatus = patchSeriesStatus;
            _getSelectSeries = getSelectSeries;
        }

        [HttpPost]
        [Route("SeriesCreate")]
        public async Task<IActionResult> Create([FromBody] SeriesCreateDto dto)
        {
            var result = await _createSeries.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("SeriesList")]
        public async Task<IActionResult> GetList([FromQuery] long business_id, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] long? usersBy = null)
        {
            var result = await _getAllSeries.ExecuteAsync(business_id,search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron series." });
            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("SeriesSelect")]
        public async Task<IActionResult> SeriesSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectSeries.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("SeriesIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long seriesId)
        {
            var result = await _getSeriesById.ExecuteAsync(seriesId);
            if (result == null)
                return NotFound(new { message = "No se encontró la serie." });

            return Ok(result);
        }

        [HttpPut]
        [Route("SeriesUpdate")]
        public async Task<IActionResult> Update([FromBody] SeriesUpdateDto dto)
        {
            var result = await _updateSeries.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("SeriesStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] SeriesStatusToggleDto dto)
        {
            var result = await _patchSeriesStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
