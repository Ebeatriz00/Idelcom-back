using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.FileTracking
{
    public class FileTrackingOpperDeleteDto
    {
        public string LinkToken { get; set; } = default!;
        public string OpporToken { get; set; } = default!;
    }

    public class FileTrackingProjectDeleteDto
    {
        public string LinkToken { get; set; } = default!;
        public string ProjectToken { get; set; } = default!;
    }
}
