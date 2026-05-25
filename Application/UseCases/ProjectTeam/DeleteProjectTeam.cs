using Application.DTOs.ProjectTeam;
using Core.Interfaces;
using FluentValidation;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.ProjectTeam
{
    public class DeleteProjectTeam
    {
        private readonly IProjectTeamRepository _repository;
        private readonly IValidator<ProjectTeamDeleteDto> _validator;

        public DeleteProjectTeam(
            IProjectTeamRepository repository,
            IValidator<ProjectTeamDeleteDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<GlobalResponse> ExecuteAsync(ProjectTeamDeleteDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            await _repository.DeleteAsync(dto.BusinessId, dto.ProjectTeamId);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Colaborador eliminado del proyecto exitosamente."
            };
        }
    }
}
