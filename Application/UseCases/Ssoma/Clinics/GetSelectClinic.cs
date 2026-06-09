using Application.DTOs.Ssoma.Clinics;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma.Clinics;

namespace Application.UseCases.Ssoma.Clinics
{
    public class GetSelectClinic(IClinicRepository repository, IMapper mapper)
    {
        private readonly IClinicRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<ClinicSelectDto?>> ExecuteAsync(long businessId, int page, int pageSize, string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);

            return new PagedSelect<ClinicSelectDto?>
            {
                Items = _mapper.Map<List<ClinicSelectDto?>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                HasMore = result.HasMore
            };
        }
    }
}
