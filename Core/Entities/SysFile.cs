using System;

namespace Core.Entities
{
    public class SysFile
    {
        public Guid Id { get; set; }
        public string RelativePath { get; set; } = default!;
        public string OriginalName { get; set; } = default!;
        public long FileSizeBytes { get; set; }
        public string MimeType { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public long CreatedByUserId { get; set; }
    }
}
