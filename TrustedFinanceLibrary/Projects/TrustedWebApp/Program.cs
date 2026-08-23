using TrustedWebApp.Helpers;

namespace TrustedWebApp
{
    // Not using top level statements for readability in the tests.
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddTrustedLibrary();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (!app.Environment.IsEnvironment("Testing"))
                app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
