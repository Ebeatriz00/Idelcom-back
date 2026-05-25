using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PreSaleProyects
{
    public class PreSaleProjectsUpdateStateDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public long? StatePreSaleId { get; set; }                                                                                       
        public long UsersBy {  get; set; }
        public int? ObsType { get; set; }
        public int? ObsSeverity { get; set; } 
        public List<string>? ObsReason { get; set; } 
        public long? ObsStatusId { get; set; }
        public DateTime? ObsDueDate { get; set; }
        public List<long>? AssignedWorkerId { get; set; } = new();
        public List<PreSaleProyectFile>? PreSaleProyectFiles { get; set; }
    }
}
