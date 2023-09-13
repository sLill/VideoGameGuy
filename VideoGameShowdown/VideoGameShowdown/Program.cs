using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VideoGameShowdown.Configuration;
using VideoGameShowdown.Core;

namespace VideoGameShowdown
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
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnectionString"));
                options.EnableSensitiveDataLogging();
            });

            var app = builder.Build();

            ConfigureApplication(app);
            ConfigureSyncfusion(app);
            ConfigureRawgApi(app);

            ImportGameData(app);

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

        private static void ImportGameData(WebApplication app)
        {
            ApplicationDbContext context = app.Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();

            var gamesList = new Dictionary<string, Game>();

            var dataFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "RAWG_Data")).ToList();
            dataFiles.ForEach(x =>
            {
                string fileRaw = File.ReadAllText(x);
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(fileRaw);

                var resultsToken = jsonObject.SelectToken("results");
                var gameList = JsonConvert.DeserializeObject<List<Game>>(resultsToken.ToString());

                if (gameList != null && gameList.Any())
                {
                    gameList.ForEach(game =>
                    {
                        if (game.EsrbRating != null)
                        {
                            var uniqueId = $"{game.EsrbRating.EsrbRatingId}-{Guid.NewGuid()}";

                            game.EsrbRatingId = uniqueId;
                            game.EsrbRating.EsrbRatingId = uniqueId;
                        }

                        gamesList[game.GameId] = game;
                    });
                }
            });

            context.AddRange(gamesList.Values);
            context.SaveChanges();
        }
        #endregion Methods..
    }
}