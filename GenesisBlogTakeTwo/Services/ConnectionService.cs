using Npgsql;

namespace GenesisBlogTakeTwo.Services
{
    public static class ConnectionService
    {
        // Classes can have a mixture of static and regular (non-static) methods.
        // However, a Static class must contain only Static methods and properties.
        public static string GetConnectionString(IConfiguration configuration)
        {
            // Get local connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Attempt to get the remote connection string
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();

        }

    }
}
