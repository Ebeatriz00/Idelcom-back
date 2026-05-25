using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comment
{
    public class CommentCreateDto
    {
        public long BusinessId { get; set; }
        public string LinkToken { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool? IsInternal { get; set; }
        public List<long> AudienceAreaIds { get; set; } = new();
        public long VisibilityId { get; set; }
        public long UsersBy { get; set; }
    }
}
