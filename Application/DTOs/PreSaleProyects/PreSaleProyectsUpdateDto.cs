using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PreSaleProyects
{
    public class PreSaleProyectsUpdateDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string Description { get; set; }
        public long ClientsId { get; set; }
        public long ContactsCrmId { get; set; }
        public long ResponsibleId { get; set; }
        public long SupervisorId { get; set; }
        public long SsomaId { get; set; }
        public long TecLeaderId { get; set; }
        public long OpportunityId { get; set; }
        public long StatePreSaleId { get; set; }
        public long QuotationNumberId { get; set; }
        public long OrderNumberId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long UsersBy { get; set; }
    }



    public class PreSaleProyectsUpdateResponsibleDto
    {
        public string LinkToken { get; set; }
        public long BusinessId { get; set; }
        public long WorkerId { get; set; }
        public int? ProjectCategory { get; set; }
        public long UsersBy { get; set; }
    }
}
