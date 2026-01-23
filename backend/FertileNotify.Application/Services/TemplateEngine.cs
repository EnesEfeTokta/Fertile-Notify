namespace FertileNotify.Application.Services
{
    public class TemplateEngine
    {
        public string Render(string template, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentException("Template cannot be null or empty.", nameof(template));
            if (parameters == null || parameters.Count == 0) return template;

            string rendered = template;
            foreach (var param in parameters)
            {
                rendered = rendered.Replace($"{{{param.Key}}}", param.Value);
            }
            return rendered;
        }
    }
}
