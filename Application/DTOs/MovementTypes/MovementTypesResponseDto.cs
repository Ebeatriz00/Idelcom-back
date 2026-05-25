using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.MovementTypes
{
    public class MovementTypesResponseDto
    {
        public long MovementTypesId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MovSunatDescription { get; set; } = string.Empty;
        public string MovOperDescription { get; set; } = string.Empty;
        public string MovPerDescription { get; set; } = string.Empty;
        public string MovClasDescription { get; set; } = string.Empty;
        public string MovVisDescription { get; set; } = string.Empty;
        public string Status { get; set; } = "1";
        public int MovemenTypesCount { get; set; } 
    }
}
