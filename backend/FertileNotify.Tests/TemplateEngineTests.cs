using FertileNotify.Application.Services;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;
using Mjml.Net;
using Moq;

namespace FertileNotify.Tests
{
    public class TemplateEngineTests
    {
        private readonly Mock<IMjmlRenderer> _mockMjmlRenderer;
        private readonly TemplateEngine _templateEngine;

        public TemplateEngineTests()
        {
            _mockMjmlRenderer = new Mock<IMjmlRenderer>();
            _templateEngine = new TemplateEngine(_mockMjmlRenderer.Object);
        }

        [Fact]
        public void Render_Should_Replace_Placeholders_In_PlainText_Channels()
        {
            // ARRANGE
            string template = "Hello {Name}, order no: {OrderId}";
            var parameters = new Dictionary<string, string>
            {
                { "Name", "Enes" },
                { "OrderId", "12345" }
            };

            // ACT - SMS gibi kanallarda MJML çalışmaz
            var result = _templateEngine.Render(template, NotificationChannel.SMS, parameters);

            // ASSERT
            result.Should().Be("Hello Enes, order no: 12345");
        }

        [Fact]
        public void Render_Should_Convert_Mjml_To_Html_For_Email_Channel()
        {
            // ARRANGE
            string mjmlTemplate = "<mjml>Hello {Name}</mjml>";
            var parameters = new Dictionary<string, string> { { "Name", "Enes" } };

            string expectedInputToRenderer = "<mjml>Hello Enes</mjml>";
            string fakeGeneratedHtml = "<html><body>Hello Enes</body></html>";

            _mockMjmlRenderer
                .Setup(x => x.Render(expectedInputToRenderer, It.IsAny<MjmlOptions>()))
                .Returns(new RenderResult(fakeGeneratedHtml, new ValidationErrors()));

            // ACT
            var result = _templateEngine.Render(mjmlTemplate, NotificationChannel.Email, parameters);

            // ASSERT
            result.Should().Be(fakeGeneratedHtml);
            _mockMjmlRenderer.Verify(x => x.Render(expectedInputToRenderer, It.IsAny<MjmlOptions>()), Times.Once);
        }

        [Fact]
        public void Render_Should_Return_Template_AsIs_When_Parameters_Are_Empty()
        {
            // ARRANGE
            string template = "Hello {Name}";
            var parameters = new Dictionary<string, string>();

            // ACT
            var result = _templateEngine.Render(template, NotificationChannel.Console, parameters);

            // ASSERT
            result.Should().Be("Hello {Name}");
        }

        [Fact]
        public void Render_Should_Handle_Null_Or_Empty_Template()
        {
            // ACT
            var result1 = _templateEngine.Render(string.Empty, NotificationChannel.Email, null!);
            var result2 = _templateEngine.Render(null!, NotificationChannel.SMS, null!);

            // ASSERT
            result1.Should().BeEmpty();
            result2.Should().BeEmpty();
        }
    }
}