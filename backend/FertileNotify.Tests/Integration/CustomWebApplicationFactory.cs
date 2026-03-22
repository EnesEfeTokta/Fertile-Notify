using FertileNotify.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using FertileNotify.Application.Interfaces;
using FertileNotify.Tests.Integration.Fakes;

namespace FertileNotify.Tests.Integration;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "integration-tests-secret-key-at-least-32-bytes-long-123456",
                ["JwtSettings:Issuer"] = "FertileNotifyAPI",
                ["JwtSettings:Audience"] = "FertileNotifyClients",
                ["JwtSettings:ExpiryInMinutes"] = "15"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));
            services.RemoveAll(typeof(ApplicationDbContext));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Replace IOtpService
            var otpDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IOtpService));
            if (otpDescriptor != null) services.Remove(otpDescriptor);
            services.AddScoped<IOtpService, FakeOtpService>();

            // Replace IEmailService
            var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor != null) services.Remove(emailDescriptor);
            services.AddScoped<IEmailService, FakeEmailService>();
        });
    }
}
