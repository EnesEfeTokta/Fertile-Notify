using FertileNotify.Domain.ValueObjects;
using Mjml.Net;

namespace FertileNotify.Application.Services
{
    public class TemplateEngine
    {
        private readonly IMjmlRenderer _mjmlRenderer;

        public TemplateEngine(IMjmlRenderer mjmlRenderer)
        {
            _mjmlRenderer = mjmlRenderer;
        }

        public string Render(string template, NotificationChannel channel, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            string rendered = template;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    rendered = rendered.Replace($"{{{param.Key}}}", param.Value ?? string.Empty);
                }
            }

            if (channel.Equals(NotificationChannel.Email))
            {
                var mjmlResult = _mjmlRenderer.Render(rendered.Trim());

                if (string.IsNullOrEmpty(mjmlResult.Html)) 
                    return rendered;
                return mjmlResult.Html;
            }

            return rendered;
        }
    }
}