using SharedKernel;

namespace Core.Entities.Operations
{
    public class OperationWorkOrderCode
    {
        public long OperationsId { get; set; }
        public int LastNumber { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "1";
    }
}
