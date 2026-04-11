using DotNetEnv;

namespace FertileNotify.API.Extensions
{
    public static class EnvExtension
    {
        public static void AddCustomEnvVariables(this IConfigurationBuilder configuration)
        {
            Env.Load();

            var envValues = new Dictionary<string, string?>();

            // DATABASE MAPPING
            string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            string? dbUser = Environment.GetEnvironmentVariable("DB_USER");
            string? dbPass = Environment.GetEnvironmentVariable("DB_PASS");

            if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbUser))
            {
                var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
                var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "FertileNotifyDb";

                envValues["ConnectionStrings:DefaultConnection"] = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass};Include Error Detail=true;";
            }

            // REDIS MAPPING
            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
            var redisPass = Environment.GetEnvironmentVariable("REDIS_PASS");

            if (!string.IsNullOrEmpty(redisHost))
            {
                var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";

                var connectionString = $"{redisHost}:{redisPort},abortConnect=false";
                if (!string.IsNullOrEmpty(redisPass))
                {
                    connectionString += $",password={redisPass}";
                }

                envValues["Redis:ConnectionString"] = connectionString;
            }

            // RABBITMQ MAPPING
            MapEnv(envValues, "RABBITMQ_HOST", "RabbitMQ:Host");
            MapEnv(envValues, "RABBITMQ_USER", "RabbitMQ:Username");
            MapEnv(envValues, "RABBITMQ_PASS", "RabbitMQ:Password");

            MapEnv(envValues, "JWT_SECRET", "JwtSettings:SecretKey");
            MapEnv(envValues, "SYSTEM_SMTP_HOST", "SystemSmtp:Host");
            MapEnv(envValues, "SYSTEM_SMTP_PORT", "SystemSmtp:Port");
            MapEnv(envValues, "SYSTEM_SMTP_USER", "SystemSmtp:Username");
            MapEnv(envValues, "SYSTEM_SMTP_PASS", "SystemSmtp:Password");

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
