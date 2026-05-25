using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FileTracking
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public string OpporToken { get; set; } = default!;
        public string ProjectToken { get; set; } = default!;
        public string? ArchiveType { get; set; }
        public string FileTitle { get; set; } 
        public string? FileUrl { get; set; }
        public string? RelativePath { get; set; }
        public string? Comment { get; set; }
        public string? CodeOppor { get; set; }
        public string? CodeProject { get; set; }
        public long UsersBy { get; set; }
        public string Status { get; set; }
    }
}
