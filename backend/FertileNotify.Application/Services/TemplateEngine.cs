using Mjml.Net;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Services
{
    public class TemplateEngine
    {
        private readonly IMjmlRenderer _mjmlRenderer = new MjmlRenderer();

        public string Render(string template, NotificationChannel channel, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            string processedText = template;
            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    processedText = processedText.Replace($"{{{param.Key}}}", param.Value);
                }
            }

            if (channel.Equals(NotificationChannel.Email))
            {
                var result = _mjmlRenderer.Render(processedText, new MjmlOptions
                {
                    Beautify = false
                });

                return result.Html;
            }

            if (channel.Equals(NotificationChannel.SMS))
            {
                return processedText;
            }

            if (channel.Equals(NotificationChannel.Console))
            {
                return processedText;
            }

            return string.Empty;
        }
    }
}