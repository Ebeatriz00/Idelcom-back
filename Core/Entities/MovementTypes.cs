using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MovementTypes
    {
        public long MovementTypesId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long MovSunatId { get; set; }
        public long MovOperId { get; set; }
        public long MovPerId { get; set; }
        public long MovClasId { get; set; }
        public long MovVisId { get; set; }

        public bool AffectsStock { get; set; }
        public bool RequiresDestWare { get; set; }
        public bool GeneratesAccounting { get; set; }
        public bool IsAdjustment { get; set; }
        public bool AllowNegative { get; set; }

        public string Status { get; set; } = "1";
        public long UsersBy { get; set; }
        public long MovemenTypesCount { get; set; }
        public string? MovSunatDescription { get; set; }
        public string? MovOperDescription { get; set; }
        public string? MovPerDescription { get; set; }
        public string? MovClasDescription { get; set; }
        public string? MovVisDescription { get; set; }
    }
}
