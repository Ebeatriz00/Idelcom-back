using Core.Entities.OperationsPersonnelAssignment;
using Core.Entities.paginations;
using Core.Interfaces.OperationsPersonnelAssignment;
using Core.Projections.OperationsPersonnelAssignment;
using Infrastructure.Exceptions;
using SharedKernel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class OperationsPersonnelAssignmentRepository : IOperationsPersonnelAssignmentRepository
    {
        private readonly IDapperHelper _dapperHelper;

        public OperationsPersonnelAssignmentRepository(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<BaseResponseId> CreateAsync(OperationPersonnelAssignment entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.SquadId,
                    entity.WorkerId,
                    entity.AssignmentDate,
                    entity.AssignmentStatusId,
                    entity.StartDate,
                    entity.FinishDate,
                    entity.Notes,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_OPERATIONS_PERSONNEL_ASSIGNMENT",
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
                throw new DatabaseException(ex.Message, ex.InnerException?.Message ?? "");
            }
        }

        public async Task<PagedResult<OperationPersonnelAssignmentProjection>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<OperationPersonnelAssignmentProjection>(
                "SP_WS_LIST_OPERATIONS_PERSONNEL_ASSIGNMENT",
                parameters);

            return new PagedResult<OperationPersonnelAssignmentProjection>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<BaseResponse> UpdateAsync(OperationPersonnelAssignment entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.AssignmentId,
                    entity.BusinessId,
                    entity.SquadId,
                    entity.WorkerId,
                    entity.AssignmentDate,
                    entity.AssignmentStatusId,
                    entity.StartDate,
                    entity.FinishDate,
                    entity.Notes,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_OPERATIONS_PERSONNEL_ASSIGNMENT",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex.InnerException?.Message ?? "");
            }
        }

        public async Task<BaseResponse> DeleteAsync(long assignmentId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    AssignmentId = assignmentId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_OPERATIONS_PERSONNEL_ASSIGNMENT",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex.InnerException?.Message ?? "");
            }
        }

        public async Task<OperationPersonnelAssignment?> GetByIdAsync(long assignmentId)
        {
            var parameters = new Dapper.DynamicParameters();
            parameters.Add("@AssignmentId", assignmentId);

            var result = await _dapperHelper.QueryFirstOrDefaultAsync<OperationPersonnelAssignment>(
                "SP_WS_OPERATIONS_PERSONNEL_ASSIGNMENT_BY_ID",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
