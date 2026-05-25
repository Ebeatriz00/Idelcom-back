using Application.DTOs.Bank;
using Application.UseCases.Bank;
using Core.Entities.paginations;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlueMark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly CreateBank _createBank;
        private readonly GetAllBank _getAllBanks;
        private readonly GetByIdBank _getByIdBank;
        private readonly UpdateBank _updateBank;
        private readonly PatchBankStatus _patchBankStatus;
        private readonly GetSelectBank _getSelectBank;

        public BankController(
            CreateBank createBank,
            GetAllBank getAllBanks,
            GetByIdBank getByIdBank,
            UpdateBank updateBank,
            PatchBankStatus patchBankStatus,
            GetSelectBank getSelectBank)
        {
            _createBank = createBank;
            _getAllBanks = getAllBanks;
            _getByIdBank = getByIdBank;
            _updateBank = updateBank;
            _patchBankStatus = patchBankStatus;
            _getSelectBank = getSelectBank;
        }

        [HttpPost]
        [Route("BankCreate")]
        public async Task<IActionResult> Create([FromBody] BankCreateDto dto)
        {
            var result = await _createBank.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Route("BankList")]
        public async Task<IActionResult> GetList(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] long? usersBy = null)
        {
            var result = await _getAllBanks.ExecuteAsync(business_id, search, page, pageSize, usersBy);

            if (result == null || !result.Items.Any())
                return NotFound(new { message = "No se encontraron bancos." });

            Response.Headers["X-Total-Count"] = result.Total.ToString();
            Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

            return Ok(result);
        }

        [HttpGet]
        [Route("BankSelect")]
        public async Task<IActionResult> BankSelect(
            [FromQuery] long business_id,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _getSelectBank.ExecuteAsync(business_id, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("BankIdList")]
        public async Task<IActionResult> GetListId([FromQuery] long BankId) 
        {
            var result = await _getByIdBank.ExecuteAsync(BankId);

            if (result == null)
                return NotFound(new { message = "No se encontró el banco solicitado." });

            return Ok(result);
        }

        [HttpPut]
        [Route("BankUpdate")]
        public async Task<IActionResult> Update([FromBody] BankUpdateDto dto)
        {
            var result = await _updateBank.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpPatch]
        [Route("BankStatus")]
        public async Task<IActionResult> Patch([FromBody] BankStatusToggleDto dto)
        {
            var result = await _patchBankStatus.ExecuteAsync(dto);
            return Ok(result);
        }
    }
}
