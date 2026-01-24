namespace FertileNotify.Application.Services
{
    public class TemplateEngine
    {
        public string Render(string template, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

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
