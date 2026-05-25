using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ContactsCrm
    {
        public long ContactsCrmId { get; set; } 
        public long BusinessId { get; set; }
        public string? ContactName { get; set; }
        public string? JobTitle { get; set; }
        public string? Phone {  get; set; }
        public string? Movil {  get; set; }
        public string? Email { get; set; }
        public long? WorkerId { get; set; }
        public long? ClientsId { get; set; }
        public long? LeadsSourcesId { get; set; }
        public long? ContactTypeId { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; } = "1";
        public string? WorkerDescritpion { get; set; }
        public string? ClientsDescription { get; set; }
        public string? LeadsSourcesDescription { get; set; }
        public string? ContactTypeDescription { get; set; }
        public int ContactsCrmCount { get; set; }

            
    }
}
