using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Clients
{
    public class ClientsByIdDto
    {
        public long ClientsId { get; set; }
        public long BusinessId { get; set; }
        public long DocumentTypeId { get; set; }
        public string Documents { get; set; }
        public string ClientsName { get; set; }
        public string? ClientsCompany { get; set; }
        public string ClientsAddress { get; set; }
        public string? ClientsPhone { get; set; }
        public long? WorkerId { get; set; }
        public long? DepartmentId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? ProcessTypeId { get; set; }
        public long? SectorId { get; set; }
        public long? LeadSourceId { get; set; }
        public long? LeadStatusId { get; set; }
        public long? LeadQualificationId { get; set; }
        public string? Website { get; set; }
        

    }
}
