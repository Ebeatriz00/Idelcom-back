using Application.DTOs.SubTasks;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.SubTasks
{
    public class DeleteSubTasks
    {
        private readonly ISubTasksRepository _repository;
        private readonly IValidator<SubTasksDeleteDto> _validator;
        private readonly ILinkTokenService _linkTokenService;

        public DeleteSubTasks(ISubTasksRepository repository, IValidator<SubTasksDeleteDto> validator, ILinkTokenService linkTokenService)
        {
            _repository = repository;
            _validator = validator;
            _linkTokenService = linkTokenService;
        }

        public async Task<GlobalResponse> ExecuteAsync(SubTasksDeleteDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                throw new AppValidationException(validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList());
            }

            if (!_linkTokenService.ValidateToken(dto.LinkToken, out _, out _, out var resourceId) || !long.TryParse(resourceId, out var subTasksId))
            {
                return new GlobalResponse { Status = 0, Message = "Token de sub-tarea inválido." };
            }

            var deleted = await _repository.DeleteAsync(subTasksId, dto.UsersBy);

            return new GlobalResponse
            {
                Status = deleted ? 1 : 0,
                Message = deleted ? "Sub-tarea eliminada correctamente." : "Error al eliminar la sub-tarea."
            };
        }
    }
}
