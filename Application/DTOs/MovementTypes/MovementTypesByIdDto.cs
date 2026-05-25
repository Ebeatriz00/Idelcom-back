using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovementTypes
{
    public class MovementTypesByIdDto
    {
        public long MovementTypesId { get; set; }
        public long BusinessId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
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

    }
}
