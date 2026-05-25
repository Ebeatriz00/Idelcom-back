using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Worker
    {
        public long WorkerId { get; set; }
        public long BusinessId { get; set; }
        public long AreaId { get; set; }
        public long JobTitleId { get; set; }
        public string? PrevJob { get; set; } = string.Empty;
        public string WorkerName { get; set; }
        public string WorkerLastName { get; set; }
        public long DocumentTypeId { get; set; }
        public string WorkerDocument { get; set; }
        public long? DepartmentId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public DateTime? DateEntry { get; set; }
        public DateTime? DateCes { get; set; } 
        public long? BankId { get; set; }
        public string? CCBank { get; set; }
        public string? CCIBank { get; set; }
        public decimal? Salary { get; set; }
        public string? NumberChildren { get; set; } = string.Empty;
        public int UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public int WorkerCount { get; set; }
       
        




        //listar
        public string WorkerFullName { get; set; }
        public string DocumentType { get; set; }
        public string District { get; set; } = string.Empty;
        public string JobTitle { get; set; }
    }
}
