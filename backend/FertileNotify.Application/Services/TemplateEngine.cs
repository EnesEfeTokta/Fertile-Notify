using System.Text;
using System.Text.RegularExpressions;
using FertileNotify.Domain.ValueObjects;
using Mjml.Net;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.Services
{
    public class TemplateEngine
    {
        private readonly IMjmlRenderer _mjmlRenderer;
        private readonly ILogger<TemplateEngine> _logger;
        private static readonly Regex TemplateVariablePattern = new(@"\{\{(\w+)\}\}", RegexOptions.Compiled);

        public TemplateEngine(IMjmlRenderer mjmlRenderer, ILogger<TemplateEngine> logger)
        {
            _mjmlRenderer = mjmlRenderer;
            _logger = logger;
        }

        public string Render(string template, NotificationChannel channel, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            string rendered = ReplaceTemplateVariables(template, parameters);

            if (channel.Equals(NotificationChannel.Email))
            {
                var mjmlResult = _mjmlRenderer.Render(rendered.Trim());

                if (mjmlResult.Errors != null && mjmlResult.Errors.Any())
                    _logger.LogWarning("MJML Rendering warnings/errors");

                return !string.IsNullOrEmpty(mjmlResult.Html) ? mjmlResult.Html : rendered;
            }

            return rendered;
        }

        private string ReplaceTemplateVariables(string template, Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return template;

            var matches = TemplateVariablePattern.Matches(template);
            if (matches.Count == 0)
                return template;

            var sb = new StringBuilder();
            int lastIndex = 0;

            foreach (Match match in matches)
            {
                // Template'in match öncesindeki kısmını ekle
                sb.Append(template, lastIndex, match.Index - lastIndex);

                // {{Key}} içindeki key'i al ve parameters'ten ara
                var key = match.Groups[1].Value;
                if (parameters.TryGetValue(key, out var value))
                {
                    sb.Append(value);
                }
                // Key bulunamazsa boş string ile değiştir

                lastIndex = match.Index + match.Length;
            }

            // Template'in kalan kısmını ekle
            sb.Append(template, lastIndex, template.Length - lastIndex);

            return sb.ToString();
        }
    }
}