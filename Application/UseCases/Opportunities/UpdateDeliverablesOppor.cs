using Application.DTOs.Opportunities;
using Core.Entities;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Opportunities
{
    public class UpdateDeliverablesOpportunities
    {
        private readonly IOpportunitiesRepository _repository;

        public UpdateDeliverablesOpportunities(IOpportunitiesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GlobalResponse> ExecuteAsync(OpportunitiesStateUpdateDto dto)
        {

            var entity = new Core.Entities.Opportunities
            {
                LinkToken = dto.LinkToken,
                BusinessId = dto.BusinessId,
                UsersBy = dto.UsersBy,

                Deliverables = dto.Deliverables?.Select(d => new DeliverablesOppor
                {
                    DeliverablesId = d.DeliverablesId, 
                    Name = d.Name,
                    Comment = d.Comment,               
                    DueDate = d.DueDate                
                }).ToList()
            };
            return await _repository.UpdateDeliverablesOnlyAsync(entity);
        }
    }
}
