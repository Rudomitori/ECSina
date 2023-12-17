using ECSina.Db;
using Microsoft.EntityFrameworkCore;

namespace ECSina.App.Setup;

public static class DbSetup
{
    public static WebApplicationBuilder SetupDb(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Default");
            optionsBuilder.UseNpgsql(connectionString);
        });

        return builder;
    }

    public static async Task ApplyMigrations(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
