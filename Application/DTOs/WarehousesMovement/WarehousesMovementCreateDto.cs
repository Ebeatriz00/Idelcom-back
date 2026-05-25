using System;
using System.Collections.Generic;

namespace Application.DTOs.WarehousesMovement
{
    public class WarehousesMovementCreateDto
    {
        public long MovementTypeId { get; set; }
        public long WarehouseId { get; set; }
        public long? WarehouseDestinationId { get; set; }
        public long? SuppliersId { get; set; }
        public long? ClientsId { get; set; }
        public string? Series { get; set; }
        public string? NumberDocument { get; set; }
        public string? ReferenceDocument { get; set; }
        public DateTime? MovementDate { get; set; }
        public string? Observation { get; set; }
        public long? TaxesId { get; set; }
        public List<WarehousesMovementDetailCreateDto> Details { get; set; } = new();
    }
}
