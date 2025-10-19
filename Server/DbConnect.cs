using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Server.Data;
namespace Server
{    public static class DbConnect
    {
        public static void AddDatabase(IServiceCollection services, IConfigurationSection config)
        {
            string? connectionString = null;
            string? server = config["Ip"];
            string? database = config["DatabaseName"];
            string? user = config["UserID"];
            string? password = config["Password"];
            string? port = config["Port"] ?? "3306";

            if (server != null && database != null && user != null && password != null)
            {
                connectionString = $"Server={server};Port={port};Database={database};User={user};Password={password};";
                services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            }
            else
            {
                throw new Exception("Database configuration is missing or incomplete.");
            }
        }
        public static bool TestDatabaseConnection(IServiceProvider serviceProvider, out Exception? exception)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                bool canConnect = dbContext.Database.CanConnect();
                exception = null;
                return canConnect;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}

