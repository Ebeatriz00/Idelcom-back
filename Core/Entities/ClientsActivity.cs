using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ClientsActivity
    {
        public long ClientsActivityId { get; set; }
        public long BusinessId { get; set; }
        public long ClientsId { get; set; }
        public long ActivityStateId { get; set; }
        public int ActivityTypeId { get; set; }
        public long WorkerId { get; set; }

        public DateTime? FinishDate { get; set; } 
        public string Description { get; set; }

        public long UsersBy { get; set; }

        public string Status { get; set; } = "1";

        public string WorkerName { get; set; }

        // Datos del Tipo (Para el icono izquierdo)
        public string Activity { get; set; }
        public string ActivityIcon { get; set; }

        // Datos del Estado (Para el badge y color)
        public string StateDesc { get; set; }
        public string StateColor { get; set; }
        public string StateIcon { get; set; }
    }
}
