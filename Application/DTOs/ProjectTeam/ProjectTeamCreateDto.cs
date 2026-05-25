using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProjectTeam
{
    public class ProjectTeamCreateDto
    {
        public long BusinessId { get; set; }
        public string ProjectToken { get; set; }
        public List<long> WorkerId { get; set; } = new();
        public long UsersBy {  get; set; }
    }
}
