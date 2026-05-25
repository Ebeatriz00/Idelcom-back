using System;

namespace Application.DTOs.Operations.OperationsWorkOrderProgress
{
    public class OperationsWorkOrderProgressPhotoDto
    {
        public Guid FileUid { get; set; }
        public string Url { get; set; } = default!;
    }
}
