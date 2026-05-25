using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Hiring
{
    public class HiringUpdateStatusDto
    {
        public long HiringId { get; set; }
        public long BusinessId { get; set; }
        public long LicStatusId { get; set; }
        public long UsersBy { get; set; }
        public List<HiringFile>? HiringFiles { get; set; }
    }
}
