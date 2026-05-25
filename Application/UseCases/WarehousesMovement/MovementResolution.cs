namespace Application.UseCases.WarehousesMovement
{
    public record MovementResolution(StockOperation StockOperation, bool AllowNegative, bool IsAdjustment);
}
