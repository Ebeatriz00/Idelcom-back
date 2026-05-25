using Application.DTOs.MedicalAptitude;
using Application.UseCases.MedicalAptitude;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalAptitudeController : ControllerBase
    {
        private readonly CreateMedicalAptitude _createMedicalAptitude;
        private readonly GetAllMedicalAptitude _getAllMedicalAptitude;
        private readonly GetByIdMedicalAptitude _getByIdMedicalAptitude;
        private readonly UpdateMedicalAptitude _updateMedicalAptitude;
        private readonly GetSelectMedicalAptitude _getSelectMedicalAptitude;
        private readonly PatchMedicalAptitude _patchMedicalAptitude;

        public MedicalAptitudeController(
            CreateMedicalAptitude createMedicalAptitude,
            GetAllMedicalAptitude getAllMedicalAptitude,
            GetByIdMedicalAptitude getByIdMedicalAptitude,
            UpdateMedicalAptitude updateMedicalAptitude,
            GetSelectMedicalAptitude getSelectMedicalAptitude,
            PatchMedicalAptitude patchMedicalAptitude)
        {
            _createMedicalAptitude = createMedicalAptitude;
            _getAllMedicalAptitude = getAllMedicalAptitude;
            _getByIdMedicalAptitude = getByIdMedicalAptitude;
            _updateMedicalAptitude = updateMedicalAptitude;
            _getSelectMedicalAptitude = getSelectMedicalAptitude;
            _patchMedicalAptitude = patchMedicalAptitude;
        }

        [HttpPost]
        [Route("MedicalAptitudeCreate")]
        public async Task<IActionResult> Create([FromBody] MedicalAptitudeCreateDto dto)
        {
            var result = await _createMedicalAptitude.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("MedicalAptitudeList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? usersBy = null)
        {
            var result = await _getAllMedicalAptitude.ExecuteAsync(business_id, search, page, pageSize, usersBy);
            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron aptitudes médicas." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("MedicalAptitudeSelect")]
        public async Task<IActionResult> Select(
            [FromQuery] long businessId,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
           
            var result = await _getSelectMedicalAptitude.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("MedicalAptitudeIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long medicalAptitudeId)
        {
            var result = await _getByIdMedicalAptitude.ExecuteAsync(medicalAptitudeId);
            if (result == null)
                return NotFound(new { message = "No se encontró la aptitud médica." });

            return Ok(result);
        }

        [HttpPut]
        [Route("MedicalAptitudeUpdate")]
        public async Task<IActionResult> Update([FromBody] MedicalAptitudeUpdateDto dto)
        {
            var result = await _updateMedicalAptitude.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("MedicalAptitudeStatus")]
        public async Task<IActionResult> PatchStatus([FromBody] MedicalAptitudeStatusToogleDto dto)
        {
            var result = await _patchMedicalAptitude.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}