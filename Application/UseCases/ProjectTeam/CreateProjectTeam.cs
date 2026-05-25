using Application.DTOs.ProfilesPermissions;
using Application.DTOs.ProjectTeam;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.ProjectTeam 
{
    public class CreateProjectTeam
    {
        private readonly IProjectTeamRepository _repository;
        private readonly IValidator<ProjectTeamCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkToken; 

        public CreateProjectTeam(
            IProjectTeamRepository repository,
            IValidator<ProjectTeamCreateDto> validator,
            IMapper mapper,
            ILinkTokenService linkToken) 
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _linkToken = linkToken; 
        }

        public async Task<GlobalResponse> ExecuteAsync(ProjectTeamCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            if (string.IsNullOrWhiteSpace(dto.ProjectToken))
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1007", "El token del proyecto es obligatorio.")
                });

            if (!_linkToken.TryValidate(dto.ProjectToken, out var entity, out var resourceId))
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1008", "Token inválido o expirado.")
                });

            if (entity != "opportunity")
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1009", "Token no pertenece a proyecto.")
                });

            long realProjectId = Convert.ToInt64(resourceId);

            if (realProjectId == 0)
            {
                throw new AppValidationException(new List<GlobalErrorDetail> {
                    new GlobalErrorDetail("1007", "El proyecto es obligatorio.")
                });
            }

            var entities = new List<Core.Entities.ProjectTeam>();

            foreach (var workerId in dto.WorkerId)
            {
                var exists = await _repository.ExistsAsync(dto.BusinessId, realProjectId, workerId);

                if (exists)
                    throw new DuplicateEntryException($"El trabajador {workerId} ya está asignado al proyecto.");

                entities.Add(new Core.Entities.ProjectTeam
                {
                    BusinessId = dto.BusinessId,
                    ProjectId = realProjectId, 
                    WorkerId = workerId,
                    UsersBy = dto.UsersBy,
                    Status = "1"
                });
            }
            await _repository.AddAsync(entities);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Colaborador(es) registrados exitosamente."
            };
        }
    }
}