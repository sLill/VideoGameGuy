using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VideoGameGuy.Configuration;
using VideoGameGuy.Core;
using VideoGameGuy.Data;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.HttpOverrides;

namespace VideoGameGuy
{
    public class Program
    {
        #region Methods..
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Settings
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Configuration.AddUserSecrets<Program>();
            builder.Services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);
            builder.Services.Configure<AzureSecretSettings>(builder.Configuration.GetSection("SecretSettings").GetSection("Azure"));
            builder.Services.Configure<RawgApiSettings>(builder.Configuration.GetSection("RawgApiSettings"));
            builder.Services.Configure<IgdbApiSettings>(builder.Configuration.GetSection("IgdbApiSettings"));

            // Logging
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddProvider(new SqlLoggerProvider((category, level) => level >= LogLevel.Error, GetMainConnectionString(builder)));
            });

            // Services
            builder.Services.AddTransient<ISecretService, SecretService>();
            builder.Services.AddSingleton<ISessionService, SessionService>();
            builder.Services.AddSingleton<ICountdownTimerService, CountdownTimerService>();
            builder.Services.AddHostedService<RawgBackgroundService>();
            builder.Services.AddHostedService<IgdbBackgroundService>();
            builder.Services.AddHttpClient();
            builder.Services.AddSignalR();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(sessionOptions => sessionOptions.IdleTimeout = TimeSpan.FromMinutes(30));

            // Data
            builder.Services.AddDbContext<MainDbContext>(options =>
            {
                options.UseSqlServer(GetMainConnectionString(builder));
                if (builder.Environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });

            builder.Services.AddDbContext<RawgDbContext>(options =>
            {
                options.UseSqlServer(GetRawgConnectionString(builder));
                if (builder.Environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });

            builder.Services.AddDbContext<IgdbDbContext>(options =>
            {
                options.UseSqlServer(GetIgdbConnectionString(builder));
                if (builder.Environment.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });

            builder.Services.AddScoped<ISystemStatusRepository, SystemStatusRepository>();
            builder.Services.AddScoped<ITrafficLogRepository, TrafficLogRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IRawgGamesRepository, RawgGamesRepository>();
            builder.Services.AddScoped<IIgdbArtworksRepository, IgdbArtworksRepository>();
            builder.Services.AddScoped<IIgdbGameModesRepository, IgdbGameModesRepository>();
            builder.Services.AddScoped<IIgdbGamesRepository, IgdbGamesRepository>();
            builder.Services.AddScoped<IIgdbMultiplayerModesRepository, IgdbMultiplayerModesRepository>();
            builder.Services.AddScoped<IIgdbPlatformFamiliesRepository, IgdbPlatformFamiliesRepository>();
            builder.Services.AddScoped<IIgdbPlatformLogosRepository, IgdbPlatformLogosRepository>();
            builder.Services.AddScoped<IIgdbPlatformsRepository, IgdbPlatformsRepository>();
            builder.Services.AddScoped<IIgdbScreenshotsRepository, IgdbScreenshotsRepository>();
            builder.Services.AddScoped<IIgdbThemesRepository, IgdbThemesRepository>();

            builder.Services.AddScoped<IIgdbGames_GameModesRepository, IgdbGames_GameModesRepository>();
            builder.Services.AddScoped<IIgdbGames_MultiplayerModesRepository, IgdbGames_MultiplayerModesRepository>();
            builder.Services.AddScoped<IIgdbGames_PlatformsRepository, IgdbGames_PlatformsRepository>();
            builder.Services.AddScoped<IIgdbGames_ThemesRepository, IgdbGames_ThemesRepository>();
            builder.Services.AddScoped<IIgdbGames_ArtworksRepository, IgdbGames_ArtworksRepository>();
            builder.Services.AddScoped<IIgdbGames_ScreenshotsRepository, IgdbGames_ScreenshotsRepository>();

            var app = builder.Build();
            var serviceScope = app.Services.CreateScope();

            ConfigureApplication(app, serviceScope);
            ConfigureSyncfusion(app);
            ConfigureRawgApi(app);
            ConfigureIgdbApi(app);

            app.Run();
        }

        private static void ConfigureApplication(WebApplication app, IServiceScope serviceScope)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.MapHub<CountdownTimerHub>("/countdownTimerHub");
            app.MapControllerRoute(name: "default",
                                   pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.MapControllerRoute(name: "ReviewScores",
            //                       pattern: "{controller=ReviewScores}/{action=Index}/{id?}");

            CheckAndPerformDatabaseMigrations(app, serviceScope);
            LogApplicationStarted(app, serviceScope);
        }

        private static void ConfigureSyncfusion(WebApplication app)
        {
            var secretService = app.Services.GetService<ISecretService>();
            string syncfusionProductKey = null;

            //if (app.Environment.IsDevelopment())
            //{
                // User-Secrets
                syncfusionProductKey = secretService.GetUserSecretSettings().SyncfusionProductKey;
            //}
            //else
            //{
            //    // Azure
            //    var azureSecretSettings = app.Services.GetRequiredService<IOptions<AzureSecretSettings>>();
            //    syncfusionProductKey = secretService.GetAzureSecret(azureSecretSettings.Value.SyncfusionProductKeyId);
            //}

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionProductKey);
        }

        private static void ConfigureRawgApi(WebApplication app)
        {
            var secretService = app.Services.GetService<ISecretService>();
            var rawgApiSettings = app.Services.GetRequiredService<IOptions<RawgApiSettings>>();

            string apiKey = null;

            //if (app.Environment.IsDevelopment())
            //{
                // User-Secrets
                apiKey = secretService.GetUserSecretSettings().RawgApiKey;
            //}
            //else
            //{
            //    // Azure
            //    var azureSecretSettings = app.Services.GetRequiredService<IOptions<AzureSecretSettings>>();
            //    apiKey = secretService.GetAzureSecret(azureSecretSettings.Value.RawgApiKeyId);
            //}

            rawgApiSettings.Value.ApiKey = apiKey;
        }

        private static void ConfigureIgdbApi(WebApplication app)
        {
            var secretService = app.Services.GetService<ISecretService>();
            var IgdbApiSettings = app.Services.GetRequiredService<IOptions<IgdbApiSettings>>();

            string clientId = null;
            string clientSecret = null;

            //if (app.Environment.IsDevelopment())
            //{
                // User-Secrets
                clientId = secretService.GetUserSecretSettings().IgdbClientId;
                clientSecret = secretService.GetUserSecretSettings().IgdbClientSecret;
            //}
            //else
            //{
            //    // Azure
            //    var azureSecretSettings = app.Services.GetRequiredService<IOptions<AzureSecretSettings>>();
            //    clientId = secretService.GetAzureSecret(azureSecretSettings.Value.IgdbClientId);
            //    clientSecret = secretService.GetAzureSecret(azureSecretSettings.Value.IgdbClientSecret);
            //}

            IgdbApiSettings.Value.ClientId = clientId;
            IgdbApiSettings.Value.ClientSecret = clientSecret;
        }

        private static void CheckAndPerformDatabaseMigrations(WebApplication app, IServiceScope serviceScope)
        {
            var logger = app.Services.GetService<ILogger<Program>>();
            var mainDbContext = serviceScope.ServiceProvider.GetRequiredService<MainDbContext>();
            var rawgDbContext = serviceScope.ServiceProvider.GetRequiredService<RawgDbContext>();
            var igdbDbContext = serviceScope.ServiceProvider.GetRequiredService<IgdbDbContext>();

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

            var igdbPendingMigrations = igdbDbContext.Database.GetPendingMigrations();
            if (igdbPendingMigrations?.Any() ?? false)
            {
                logger.LogInformation($"Performing db migrations for {igdbDbContext.Database.GetDbConnection().Database}");
                igdbDbContext.Database.Migrate();
            }
        }

        private static string GetMainConnectionString(WebApplicationBuilder builder)
        {
            string connectionString = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                connectionString = builder.Configuration.GetConnectionString("ConnectionString_Main");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                connectionString = builder.Configuration["ConnectionString_Main"];

            return connectionString;
        }

        private static string GetRawgConnectionString(WebApplicationBuilder builder)
        {
            string connectionString = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                connectionString = builder.Configuration.GetConnectionString("ConnectionString_Rawg");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                connectionString = builder.Configuration["ConnectionString_Rawg"];

            return connectionString;
        }

        private static string GetIgdbConnectionString(WebApplicationBuilder builder)
        {
            string connectionString = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                connectionString = builder.Configuration.GetConnectionString("ConnectionString_Igdb");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                connectionString = builder.Configuration["ConnectionString_Igdb"];

            return connectionString;
        }

        private static void LogApplicationStarted(WebApplication app, IServiceScope serviceScope)
        {
            var logger = app.Services.GetService<ILogger<Program>>();
            var systemStatusRepository = serviceScope.ServiceProvider.GetRequiredService<ISystemStatusRepository>();

            logger.LogInformation("Application Started");
         
            var systemStatus = systemStatusRepository.GetCurrentStatusAsync().Result;
            systemStatus.Application_StartedOnUtc = DateTime.UtcNow;
            systemStatusRepository.UpdateAsync(systemStatus).Wait();
        }
        #endregion Methods..
    }
}