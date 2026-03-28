using FertileNotify.Application.Services;
using Xunit;

namespace FertileNotify.Tests
{
    public class WorkflowCronSchedulingTests
    {
        [Fact]
        public void GetNextRun_With_Valid_Cron_Should_Return_Date()
        {
            // Arrange
            const string cronExpression = "0 9 * * *";

            // Act
            var result = AutomationSchedulerService.GetNextRun(cronExpression);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetNextRun_With_Invalid_Cron_Should_Return_Null()
        {
            // Arrange
            const string invalidCronExpression = "invalid";

            // Act
            var result = AutomationSchedulerService.GetNextRun(invalidCronExpression);

            // Assert
            Assert.Null(result);
        }
    }
}
