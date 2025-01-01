using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class MainClass
{
    private static void Main(string[] args)
    {
        Sqlite.InitializeDatabase();
        var port = Environment.GetEnvironmentVariable("PORT") ?? "6005";
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                    {
                        services.AddCors(options =>
                        {
                            options.AddPolicy("AllowSpecificOrigins", policy =>
                            {
                                policy.AllowAnyOrigin().WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                                    .WithHeaders("Content-Type");
                            });
                        });
                    }).Configure(app =>
                    {
                        app.UseCors("AllowSpecificOrigins");
                        app.Run(async context =>
                        {
                            await User.UserHandler(context);
                        });
                    })
                    .UseKestrel()
                    .UseUrls($"http://0.0.0.0:{port}");
            }).Build().Run();
    }
}