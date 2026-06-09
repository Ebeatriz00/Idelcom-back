using Application.DTOs.Ssoma.Clinics;
using Application.UseCases.Ssoma.Clinics;
using Idelcom.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers.Ssoma
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClinicController(
        CreateClinic createClinic,
        UpdateClinic updateClinic,
        DeleteClinic deleteClinic,
        GetAllClinics getAllClinics,
        GetByIdClinic getByIdClinic,
        GetSelectClinic getSelectClinic) : BaseController
    {
        private readonly CreateClinic _createClinic = createClinic;
        private readonly UpdateClinic _updateClinic = updateClinic;
        private readonly DeleteClinic _deleteClinic = deleteClinic;
        private readonly GetAllClinics _getAllClinics = getAllClinics;
        private readonly GetByIdClinic _getByIdClinic = getByIdClinic;
        private readonly GetSelectClinic _getSelectClinic = getSelectClinic;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getAllClinics.ExecuteAsync(businessId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Select(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getSelectClinic.ExecuteAsync(businessId, page, pageSize, search);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long clinicId)
        {
            var businessId = GetCurrentBusinessId();
            var result = await _getByIdClinic.ExecuteAsync(clinicId, businessId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClinicCreateDto dto)
        {
            dto.BusinessId = GetCurrentBusinessId();
            dto.CreateUser = GetCurrentUserId();
            var result = await _createClinic.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ClinicUpdateDto dto)
        {
            dto.BusinessId = GetCurrentBusinessId();
            dto.UpdateUser = GetCurrentUserId();
            var result = await _updateClinic.ExecuteAsync(dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long clinicId)
        {
            var userId = GetCurrentUserId(); 
            var result = await _deleteClinic.ExecuteAsync(clinicId, userId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
