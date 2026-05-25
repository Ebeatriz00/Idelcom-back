using Core.Entities.OperationsPersonnelMovement;
using Core.Entities.paginations;
using Core.Interfaces.OperationsPersonnelMovement;
using Core.Projections.OperationsPersonnelMovement;
using Infrastructure.Exceptions;
using SharedKernel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class OperationsPersonnelMovementRepository : IOperationsPersonnelMovementRepository
    {
        private readonly IDapperHelper _dapperHelper;

        public OperationsPersonnelMovementRepository(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<BaseResponseId> CreateAsync(OperationPersonnelMovement entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.WorkerId,
                    entity.FromOperationsId,
                    entity.ToOperationsId,
                    entity.FromSquadId,
                    entity.ToSquadId,
                    entity.MovementDate,
                    entity.ReleaseTime,
                    entity.ReassignmentTime,
                    entity.MovementStatusId,
                    entity.MovementReason,
                    entity.AuthorizedBy,
                    entity.RegisteredBy,
                    entity.RegularizedBy,
                    entity.RegularizedDate,
                    entity.Observation,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_OPERATIONS_PERSONNEL_MOVEMENT",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex.StackTrace);
            }
        }

        public async Task<PagedResult<OperationsPersonnelMovementProjection>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<OperationsPersonnelMovementProjection>(
                "SP_WS_LIST_OPERATIONS_PERSONNEL_MOVEMENT",
                parameters);

            return new PagedResult<OperationsPersonnelMovementProjection>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }
    }
}
