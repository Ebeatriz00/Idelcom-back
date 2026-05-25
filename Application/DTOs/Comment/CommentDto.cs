using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comment
{
    public class CommentDto
    {
        public string CommentToken { get; set; }
        public long BusinessId { get; set; }
        public string LinkToken { get; set; }
        public string Message { get; set; } = string.Empty;
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<long> AudienceAreaIds { get; set; } = new();
        public long VisibilityId { get; set; }
        public string? CreatedByName { get; set; }
    }
    public class AreaAudienceDto
    {
        public long AreaId { get; set; }
    }
}
