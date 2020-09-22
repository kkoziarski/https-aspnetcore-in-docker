using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HttpsAspNetCore
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
                    webBuilder.UseStartup<Startup>()
                    ////.UseUrls("https://+:443;http://+:80")
                    .UseKestrel(options =>
                    {
                        var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));
                        var httpsPort = configuration.GetValue("ASPNETCORE_HTTPS_PORT", 44388); // takes from environment
                        var certPassword = configuration.GetValue<string>("CertPassword"); // takes from environment
                        var certPath = configuration.GetValue<string>("CertPath"); //takes from environment

                        Console.WriteLine($"{nameof(httpsPort)}: {httpsPort}");
                        Console.WriteLine($"{nameof(certPassword)}: {certPassword}");
                        Console.WriteLine($"{nameof(certPath)}: {certPath}");

                        //IPAddress.Loopback doesn't work in Docker
                        options.Listen(IPAddress.Any, httpsPort, listenOptions =>
                        {
                            listenOptions.UseHttps(certPath, certPassword);
                        });
                    });
                });
    }
}
