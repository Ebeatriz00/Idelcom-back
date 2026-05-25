using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ClientsActivity
{
    public class ClientsActivityResponseDto
    {
        public long ClientsActivityId { get; set; }
        public long ClientsId { get; set; }
        public string Description { get; set; }
        public DateTime? FinishDate { get; set; }
        public string WorkerName { get; set; } 
        public string Activity { get; set; }     
        public string ActivityIcon { get; set; } 
        public string StateDesc { get; set; }     
        public string StateColor { get; set; }   
        public string StateIcon { get; set; }   
    }
}
