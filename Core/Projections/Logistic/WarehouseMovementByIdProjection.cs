namespace Core.Projections.Logistic
{
    public class WarehouseMovementByIdProjection
    {
        public WarehouseMovementHeaderProjection? Header { get; set; }
        public IReadOnlyList<WarehouseMovementDetailProjection> Details { get; set; } = Array.Empty<WarehouseMovementDetailProjection>();
    }
}
