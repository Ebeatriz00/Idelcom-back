using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Worker
{
    public class WorkerUpdateDto
    {
            public long WorkerId { get; set; }
            public long BusinessId { get; set; }
            public long AreaId { get; set; }
            public long JobTitleId { get; set; }
            public string? PrevJob { get; set; }
            public string WorkerName { get; set; } = string.Empty;
            public string WorkerLastName { get; set; } = string.Empty;
            public long DocumentTypeId { get; set; }
            public string WorkerDocument { get; set; } = string.Empty;
            public long? DepartmentId { get; set; }
            public long? ProvinceId { get; set; }
            public long? DistrictId { get; set; }
            public string? Address { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public DateTime? BirthDate { get; set; }
            public DateTime? DateEntry { get; set; }
            public DateTime? DateCes { get; set; }
            public long? BankId { get; set; }
            public string? CCBank { get; set; }
            public string? CCIBank { get; set; }
            public decimal? Salary { get; set; }
            public int? NumberChildren { get; set; }
        
            public int UsersBy { get; set; }
        }

    }
