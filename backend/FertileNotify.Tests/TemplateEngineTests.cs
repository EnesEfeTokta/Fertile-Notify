using FertileNotify.Application.Services;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class TemplateEngineTests
    {
        [Fact]
        public void Render_Should_Replace_Placeholders_In_PlainText_Channels()
        {
            // ARRANGE
            var engine = new TemplateEngine();
            string template = "Hello {Name}, order no: {OrderId}";
            var parameters = new Dictionary<string, string>
            {
                { "Name", "Enes" },
                { "OrderId", "12345" }
            };

            // ACT - SMS veya Console gibi kanallarda MJML çalışmaz, düz metin döner.
            var result = engine.Render(template, NotificationChannel.SMS, parameters);

            // ASSERT
            result.Should().Be("Hello Enes, order no: 12345");
        }

        [Fact]
        public void Render_Should_Convert_Mjml_To_Html_For_Email_Channel()
        {
            // ARRANGE
            var engine = new TemplateEngine();
            // Basit bir MJML şablonu
            string mjmlTemplate = "<mjml><mj-body><mj-section><mj-column><mj-text>Hello {Name}</mj-text></mj-column></mj-section></mj-body></mjml>";
            var parameters = new Dictionary<string, string>
            {
                { "Name", "Enes" }
            };

            // ACT
            var result = engine.Render(mjmlTemplate, NotificationChannel.Email, parameters);

            // ASSERT
            result.Should().Contain("<!doctype html>");
            result.Should().Contain("Hello Enes");
            result.Should().Contain("<body");
        }

        [Fact]
        public void Render_Should_Return_Template_AsIs_When_Parameters_Are_Empty()
        {
            // ARRANGE
            var engine = new TemplateEngine();
            string template = "Hello {Name}";
            var parameters = new Dictionary<string, string>();

            // ACT
            var result = engine.Render(template, NotificationChannel.Console, parameters);

            // ASSERT
            result.Should().Be("Hello {Name}");
        }

        [Fact]
        public void Render_Should_Handle_Null_Or_Empty_Template()
        {
            // ARRANGE
            var engine = new TemplateEngine();

            // ACT
            var result1 = engine.Render(string.Empty, NotificationChannel.Email, null!);
            var result2 = engine.Render(null!, NotificationChannel.SMS, null!);

            // ASSERT
            result1.Should().BeEmpty();
            result2.Should().BeEmpty();
        }
    }
}