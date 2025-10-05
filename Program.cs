using ABC_Retail.Models;
using ABC_Retail.Services;

namespace ABC_Retail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC
            builder.Services.AddControllersWithViews();

            // Bind StorageOptions from appsettings.json
            builder.Services.Configure<StorageOptions>(
                builder.Configuration.GetSection("StorageOptions"));

            // Get options instance (so we can pass to singletons)
            var storage = builder.Configuration
                .GetSection("StorageOptions")
                .Get<StorageOptions>();

            // Register storage services
            builder.Services.AddSingleton(new TableStorageService(storage));
            builder.Services.AddSingleton(new BlobStorageService(storage));
            builder.Services.AddSingleton(new FileShareStorageService(storage));
            builder.Services.AddSingleton(new QueueStorageService(storage.ConnectionString, storage.QueueName));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
