using Core.Interfaces.Notifications;
using RazorLight;

namespace Infrastructure.Notifications
{
    public sealed class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly RazorLightEngine _razor;

        public EmailTemplateRenderer()
        {
            var asm = typeof(EmailTemplateRenderer).Assembly;

            _razor = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(asm, "Infrastructure.EmailTemplates")
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderAsync(string templateName, object model)
        {
            var key = $"{templateName}.cshtml";
            //Console.WriteLine($"[TPL] Compilando template: {key}");

            try
            {
                var result = await _razor.CompileRenderAsync(key, model);
                return result;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[TPL ERROR] {ex.Message}");
                //// Log detallado para debugging
                //Console.WriteLine($"[TPL DEBUG] Assembly: {typeof(EmailTemplateRenderer).Assembly.FullName}");
                //Console.WriteLine($"[TPL DEBUG] Resource Names:");
                foreach (var resource in typeof(EmailTemplateRenderer).Assembly.GetManifestResourceNames())
                {
                    Console.WriteLine($"  - {resource}");
                }
                throw;
            }
        }
    }
}