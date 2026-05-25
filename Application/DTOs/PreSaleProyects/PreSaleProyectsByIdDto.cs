using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PreSaleProyects
{
    public class PreSaleProyectsByIdDto
    {
        public string LinkToken { get; set; }

        public long BusinessId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } 

        public long ClientsId { get; set; }
        public long ResponsibleId { get; set; }
        public long SupervisorId { get; set; }
        public long SsomaId { get; set; }
        public long TecLeaderId { get; set; }
        public long SellerId { get; set; }

        public long? StatePreSaleId { get; set; }

        public string StateColor { get; set; }
        public string StateGeneralDesc { get; set; }
        public string StateGeneralColor { get; set; }
        public string OpportunityStateDesc { get; set; }
        public string OpportunityStateColor { get; set; }
        public string OpportunityNumber { get; set; }

        public DateTime? FinishDate { get; set; }
        public int NumPercPro { get; set; }
        public int PreSaleProyectsCount { get; set; }
    }
}
