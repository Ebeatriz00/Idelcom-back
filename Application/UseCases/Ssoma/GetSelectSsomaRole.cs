using Application.DTOs.Ssoma;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces.Ssoma;

namespace Application.UseCases.Ssoma
{
    public class GetSelectSsomaRole(ISsomaRoleRepository repository, IMapper mapper)
    {
        private readonly ISsomaRoleRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedSelect<SsomaRoleSelectDto?>> ExecuteAsync(long businessId, int page, int pageSize, string? search)
        {
            var result = await _repository.GetForSelectAsync(businessId, page, pageSize, search);

            return new PagedSelect<SsomaRoleSelectDto?>
            {
                Items = _mapper.Map<List<SsomaRoleSelectDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                HasMore = result.HasMore
            };
        }
    }
}
