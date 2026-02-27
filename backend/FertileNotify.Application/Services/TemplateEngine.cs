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
                    rendered = rendered.Replace($"{{{param.Key}}}", param.Value);
                }
            }

            if (channel.Equals(NotificationChannel.Email))
            {
                var result = _mjmlRenderer.Render(rendered);
                return result.Html;
            }

            // Telegram, Console ve SMS buradan geçer ve değişkenleri dolmuş halde döner.
            return rendered;
        }
    }
}