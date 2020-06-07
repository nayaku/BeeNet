using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeNetServer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeeNetServer
{
    public class Program
    {
        public static IHost IHost { get; private set; }
        public static void Main(string[] args)
        {
            // 初始化设置读取器
            var k = UserSettingReader.UserSettings;
            var host = IHost = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);
            host.Run();
            //CreateHostBuilder(args).Build().Run();
        }
        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            using var context = scope.ServiceProvider.GetRequiredService<BeeNetContext>();
            context.Database.Migrate();

        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
