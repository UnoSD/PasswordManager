using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args) =>
            new WebHostBuilder()
                .UseKestrel(k => 
                    k.Listen(IPAddress.Loopback, 5000, o => 
                        o.UseHttps("Certificate.pfx", "paccia")))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
}