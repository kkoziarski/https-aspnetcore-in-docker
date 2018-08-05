using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HttpsAspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
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
                        //var certificate = new X509Certificate2(certPath, certPassword);
                        //var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                        //{
                        //    ClientCertificateMode = ClientCertificateMode.NoCertificate,
                        //    SslProtocols = System.Security.Authentication.SslProtocols.Tls,
                        //    ServerCertificate = certificate
                        //};
                        //listenOptions.UseHttps(httpsConnectionAdapterOptions);
                        listenOptions.UseHttps(certPath, certPassword);
                    });
                });
    }
}
