using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProjectTeam
    {
        public long ProjectTeamId { get; set; }
        public long BusinessId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectToken { get; set; }
        public long WorkerId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";

        public string WorkerName { get; set; }
        public string JobTitle { get; set; }
    }
}
