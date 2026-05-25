using Application.DTOs.Operations;
using Application.DTOs.OperationsSquad;
using AutoMapper;
using Core.Entities.OperationsSquad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.OperationsSquad
{
    public class OperationsSquadProfile : Profile 
    {
        public  OperationsSquadProfile()
        {
            CreateMap<OperationsSquadCreateDto, OperationSquad> ();
            CreateMap<OperationSquad, OperationsSquadResponseDto> ();
            CreateMap<OperationsSquadUpdateDto, OperationSquad>();
        }
    }
}
