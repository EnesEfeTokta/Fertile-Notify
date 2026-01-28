using FertileNotify.Application.Services;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class TemplateEngineTests
    {
        [Fact]
        public void Render_Should_Replace_Placeholders_With_Values()
        {
            var engine = new TemplateEngine();

            string template = "Hello {Name}, order no: {OrderId}";

            var parameters = new Dictionary<string, string>
            {
                { "Name", "Enes" },
                { "OrderId", "12345" }
            };

            var result = engine.Render(template, parameters);

            result.Should().Be("Hello Enes, order no: 12345");
        }

        [Fact]
        public void Render_Should_Return_Template_AsIs_When_Parameters_Are_Empty()
        {
            var engine = new TemplateEngine();
            string template = "Hello {Name}";
            var parameters = new Dictionary<string, string>();

            var result = engine.Render(template, parameters);

            result.Should().Be("Hello {Name}");
        }

        [Fact]
        public void Render_Should_Handle_Null_Template()
        {
            var engine = new TemplateEngine();
            var result = engine.Render(string.Empty, new Dictionary<string, string>());
            result.Should().BeEmpty();
        }
    }
}