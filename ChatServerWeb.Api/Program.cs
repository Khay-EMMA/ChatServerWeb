using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerWeb.BusinessLogic;
using ChatServerWeb.BusinessLogic.Service;
using ChatServerWeb.BusinessLogic.TCPServer;
using ChatServerWeb.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChatServerWeb.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var host = CreateWebHostBuilder(args).Build();

            //Application Initializer

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    ApplicationInitializer applicationInitializer = services.GetRequiredService<ApplicationInitializer>();
                    ChatMessageSingletonService chatMessageSingletonService = services.GetRequiredService<ChatMessageSingletonService>();
                    IMapper mapper = services.GetRequiredService<IMapper>();
                    applicationInitializer.InitializeSingleton();
                    //Start TCP Server
                    General.InitServer(chatMessageSingletonService, mapper, services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");

                }
            }


            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
