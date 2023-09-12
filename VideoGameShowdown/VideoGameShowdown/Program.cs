using Microsoft.Extensions.Options;
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
            builder.Services.AddTransient<IRawgApiService, RawgApiService>();
            builder.Services.AddHostedService<RawgBackgroundService>();
            builder.Services.AddHttpClient();
            builder.Services.AddControllersWithViews();

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
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
        #endregion Methods..
    }
}