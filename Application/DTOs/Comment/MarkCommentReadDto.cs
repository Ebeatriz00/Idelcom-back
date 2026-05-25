using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comment
{
    public class MarkCommentReadDto
    {
        public long BusinessId { get; set; }
        public long CreatedBy { get; set; }
        public string LinkToken { get; set; } = string.Empty;
    }
}
