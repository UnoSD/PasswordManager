using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args) =>
            new WebHostBuilder().UseKestrel()
                                .UseContentRoot(Directory.GetCurrentDirectory())
                                .UseStartup<Startup>()
                                .UseKestrel(k => k.UseHttps(@"Certificate.pfx", "paccia"))
                                .UseUrls("https://localhost:5000")
                                .Build()
                                .Run();
    }
}
