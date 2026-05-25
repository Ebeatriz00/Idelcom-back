using Application.DTOs.Comment;
using Application.UseCases.Comment;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Idelcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly GetComment _getComment;
        private readonly CreateComment _createComment;
        private readonly MarkCommentRead _markCommentRead;
        private readonly ILinkTokenService _linkToken;
        public CommentController(
            GetComment getComment,
            CreateComment createComment,
            ILinkTokenService linkToken,
            MarkCommentRead markCommentRead)
        {
            _getComment = getComment;
            _createComment = createComment;
            _linkToken = linkToken;
            _markCommentRead = markCommentRead;
        }
        [HttpGet("List")]
        public async Task<IActionResult> List(
        [FromQuery] long businessId,
        [FromQuery] string linkToken,
        [FromQuery] long usersId,
        [FromQuery] long areaId,
        [FromQuery] long userInternalVisibilityId)
        {
            if (!_linkToken.TryValidate(linkToken, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (entity != "opportunity")
                return BadRequest("Token no pertenece a Oportunidades.");

            var LinkToken = Convert.ToString(resourceId);

            var items = await _getComment.ExecuteAsync(businessId, LinkToken, usersId, areaId, userInternalVisibilityId);
            return Ok(items);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");


            var created = await _createComment.ExecuteAsync(dto);
            return Ok(created);
        }
        [HttpPost("MarkRead")]

        public async Task<IActionResult> MarkRead([FromBody] MarkCommentReadDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LinkToken))
                return Unauthorized("Falta token.");

            if (!_linkToken.ValidateToken(dto.LinkToken, out var claims, out var entity, out var resourceId))
                return Unauthorized("Token inválido o expirado.");

            if (!string.Equals(entity, "opportunity", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Token no pertenece a oportunidad.");

            dto.LinkToken = resourceId;

            await _markCommentRead.ExecuteAsync(dto);
            return Ok();
        }
    }
}
