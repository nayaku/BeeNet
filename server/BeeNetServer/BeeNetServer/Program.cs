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
            _ = UserSettingReader.UserSettings;
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

            var trExist = context.Database.ExecuteSqlRaw("SELECT 1 FROM sqlite_master where type='trigger' and name='TrPictureLabelInsert' LIMIT 1");
            if (trExist == 0)
                context.Database.ExecuteSqlRaw("CREATE TRIGGER TrPictureLabelInsert AFTER INSERT ON PictureLabels BEGIN UPDATE Labels SET Num=Num+1 WHERE Labels.Name=NEW.LabelName; END; ");
            trExist = context.Database.ExecuteSqlRaw("SELECT 1 FROM sqlite_master where type='trigger' and name='TrPictureLabelDelete' LIMIT 1");
            if(trExist == 0)
                context.Database.ExecuteSqlRaw("CREATE TRIGGER TrPictureLabelDelete AFTER DELETE ON PictureLabels BEGIN UPDATE Labels SET Num = Num - 1 WHERE Name = NEW.LabelName; END; ")
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        public static void CreateTrigger()
        {

        }
    }
}
