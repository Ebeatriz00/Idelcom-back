using Application.DTOs.Operations.MovementStatus;
using AutoMapper;
using Core.Entities.Operations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class MovementStatusProfile : Profile
    {
        public MovementStatusProfile()
        {
            CreateMap<MovementStatusSelectItem, MovementStatusGetSelectDto>();
            CreateMap<MovementStatus, MovementStatusGetByIdDto>();
        }
    }
}
