using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Notifications
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, object model);
    }
}
