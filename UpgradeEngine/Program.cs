using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APIGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                //var CurrentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var CurrentEnvironment = "Development";
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureAppConfiguration(config => config.AddJsonFile($"Ocelot.{CurrentEnvironment}.json"));

            }).ConfigureLogging(Logging => Logging.AddConsole());
    }
}
