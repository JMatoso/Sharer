using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sharer.Services;

namespace Sharer
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
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(IPManager.GenAppRunAddress());
                });
    }
}
