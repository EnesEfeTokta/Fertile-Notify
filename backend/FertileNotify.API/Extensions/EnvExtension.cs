using DotNetEnv;

namespace FertileNotify.API.Extensions
{
    public static class EnvExtension
    {
        public static void AddCustomEnvVariables(this IConfigurationBuilder configuration)
        {
            Env.Load();

            var envValues = new Dictionary<string, string?>();

            // JWT Mapping
            MapEnv(envValues, "JWT_SECRET", "JwtSettings:SecretKey");

            // Database Mapping
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            if (!string.IsNullOrEmpty(dbUser) && !string.IsNullOrEmpty(dbPass))
            {
                var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
                var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "FertileNotifyDb";
                envValues["ConnectionStrings:DefaultConnection"] = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass};";
            }

            // Redis Mapping
            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
            if (!string.IsNullOrEmpty(redisHost))
            {
                var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
                envValues["Redis:ConnectionString"] = $"{redisHost}:{redisPort},abortConnect=false";
            }

            // RabbitMQ Mapping
            MapEnv(envValues, "RABBITMQ_HOST", "RabbitMQ:Host");
            MapEnv(envValues, "RABBITMQ_USER", "RabbitMQ:Username");
            MapEnv(envValues, "RABBITMQ_PASS", "RabbitMQ:Password");

            // OTP Mapping
            MapEnv(envValues, "OTP_LENGTH", "OTPSettings:Length");
            MapEnv(envValues, "OTP_EXPIRY_IN_MINUTES", "OTPSettings:ExpiryInMinutes");

            // SMTP Mapping
            MapEnv(envValues, "SYSTEM_SMTP_HOST", "SystemSmtp:Host");
            MapEnv(envValues, "SYSTEM_SMTP_PORT", "SystemSmtp:Port");
            MapEnv(envValues, "SYSTEM_SMTP_USER", "SystemSmtp:Username");
            MapEnv(envValues, "SYSTEM_SMTP_PASS", "SystemSmtp:Password");
            MapEnv(envValues, "SYSTEM_SMTP_FROM", "SystemSmtp:From");
            MapEnv(envValues, "SYSTEM_SMTP_DISPLAY_NAME", "SystemSmtp:DisplayName");

            // SECURITY Mapping
            MapEnv(envValues, "SECURITY_UNSUBSCRIBE_SECRET", "Security:UnsubscribeSecret");

            configuration.AddInMemoryCollection(envValues);
        }

        private static void MapEnv(Dictionary<string, string?> dict, string envKey, string configKey)
        {
            var val = Environment.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrEmpty(val))
            {
                dict[configKey] = val;
            }
        }
    }
}
