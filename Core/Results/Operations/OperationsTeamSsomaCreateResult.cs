using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results.Operations
{
    public class OperationsTeamSsomaCreateResult
    {
        public GlobalResponse Response { get; set; } = default!;
        public List<OperationsTeamSsomaCreatedItem> InsertedRows { get; set; } = new();
    }

    public class OperationsTeamSsomaCreatedItem
    {
        public long OperationsTeamSsomaId { get; set; }
        public long WorkerId { get; set; }
        public int SsomaRoleId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; } 
        public bool IsPrimary { get; set; }
    }
}
