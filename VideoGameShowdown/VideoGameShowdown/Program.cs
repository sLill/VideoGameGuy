using Microsoft.Extensions.Options;
using VideoGameShowdown.Configuration;
using VideoGameShowdown.Core;

namespace VideoGameShowdown
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json");

            // Settings
            builder.Services.Configure<SecretSettings>(builder.Configuration.GetSection("SecretSettings"));
            builder.Services.Configure<RawgApiSettings>(builder.Configuration.GetSection("RawgApiSettings"));

            // Services
            builder.Services.AddTransient<ISecretService, SecretService>();
            builder.Services.AddTransient<IRawgApiService, RawgApiService>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            
           ConfigureApplication(app);
           ConfigureSyncfusion(app);

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
            // License
            var secretService = app.Services.GetService<ISecretService>();
            var secretSettings = app.Services.GetRequiredService<IOptions<SecretSettings>>();

            string syncfusionProductKey = secretService.GetAzureSecret(secretSettings.Value.Azure_SyncfusionProductKey);
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionProductKey);
        }
    }
}