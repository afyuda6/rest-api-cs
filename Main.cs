using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                webBuilder.UseKestrel()
                    .Configure(app =>
                    {
                        app.Run(async context =>
                        {
                            await User.UserHandler(context);
                        });
                    })
                    .UseUrls($"http://0.0.0.0:{port}");
            }).Build().Run();
    }
}