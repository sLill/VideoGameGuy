using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VideoGameCritic.Configuration;
using VideoGameCritic.Core;
using VideoGameCritic.Data;

namespace VideoGameCritic
{
    public class Program
    {
        #region Methods..
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Settings
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Configuration.AddUserSecrets<Program>();
            builder.Services.Configure<AzureSecretSettings>(builder.Configuration.GetSection("SecretSettings").GetSection("Azure"));
            builder.Services.Configure<RawgApiSettings>(builder.Configuration.GetSection("RawgApiSettings"));

            // Services
            builder.Services.AddTransient<ISecretService, SecretService>();
            builder.Services.AddHostedService<RawgBackgroundService>();
            builder.Services.AddHttpClient();
            builder.Services.AddControllersWithViews();

            // Data
            builder.Services.AddDbContext<MainDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString_Dev_Main"));
                if (builder.Environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();

            }, ServiceLifetime.Singleton);

            builder.Services.AddDbContext<RawgDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString_Dev_Rawg"));
                if (builder.Environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();

            }, ServiceLifetime.Singleton);

            builder.Services.AddSingleton<ISystemStatusRepository, SystemStatusRepository>();
            builder.Services.AddSingleton<IGamesRepository, GamesRepository>();

            var app = builder.Build();

            ConfigureApplication(app);
            ConfigureSyncfusion(app);
            ConfigureRawgApi(app);

            app.Run();
        }

        private static void ConfigureApplication(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.MapControllerRoute(name: "ReviewScores",
            //                       pattern: "{controller=ReviewScores}/{action=Index}/{id?}");

            CheckAndPerformDatabaseMigrations(app);
            LogApplicationStarted(app);
        }

        private static void ConfigureSyncfusion(WebApplication app)
        {
            var secretService = app.Services.GetService<ISecretService>();
            string syncfusionProductKey = null;

            if (app.Environment.IsDevelopment())
            {
                // User-Secrets
                syncfusionProductKey = secretService.GetUserSecretSettings().SyncfusionProductKey;
            }
            else
            {
                // Azure
                var azureSecretSettings = app.Services.GetRequiredService<IOptions<AzureSecretSettings>>();
                syncfusionProductKey = secretService.GetAzureSecret(azureSecretSettings.Value.SyncfusionProductKeyId);
            }

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionProductKey);
        }

        private static void ConfigureRawgApi(WebApplication app)
        {
            var secretService = app.Services.GetService<ISecretService>();
            var rawgApiSettings = app.Services.GetRequiredService<IOptions<RawgApiSettings>>();

            string rawgApiKey = null;

            if (app.Environment.IsDevelopment())
            {
                // User-Secrets
                rawgApiKey = secretService.GetUserSecretSettings().RawgApiKey;
            }
            else
            {
                // Azure
                var azureSecretSettings = app.Services.GetRequiredService<IOptions<AzureSecretSettings>>();
                rawgApiKey = secretService.GetAzureSecret(azureSecretSettings.Value.RawgApiKeyId);
            }

            rawgApiSettings.Value.ApiKey = rawgApiKey;
        }

        private static void CheckAndPerformDatabaseMigrations(WebApplication app)
        {
            var logger = app.Services.GetService<ILogger<Program>>();
            var mainDbContext = app.Services.GetService<MainDbContext>();
            var rawgDbContext = app.Services.GetService<RawgDbContext>();

            var mainPendingMigrations = mainDbContext.Database.GetPendingMigrations();
            if (mainPendingMigrations?.Any() ?? false)
            {
                logger.LogInformation($"Performing db migrations for {mainDbContext.Database.GetDbConnection().Database}");
                mainDbContext.Database.Migrate();
            }

            var rawgPendingMigrations = rawgDbContext.Database.GetPendingMigrations();
            if (rawgPendingMigrations?.Any() ?? false)
            {
                logger.LogInformation($"Performing db migrations for {rawgDbContext.Database.GetDbConnection().Database}");
                rawgDbContext.Database.Migrate();
            }
        }

        private static void LogApplicationStarted(WebApplication app)
        {
            var logger = app.Services.GetService<ILogger<Program>>();
            var systemStatusRepository = app.Services.GetService<ISystemStatusRepository>();

            logger.LogInformation("Application Started");
         
            var systemStatus = systemStatusRepository.GetCurrentStatusAsync().Result;
            systemStatus.Application_StartedOnUtc = DateTime.UtcNow;
            systemStatusRepository.UpdateAsync(systemStatus).Wait();
        }
        #endregion Methods..
    }
}