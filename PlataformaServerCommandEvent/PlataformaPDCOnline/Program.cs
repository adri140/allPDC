using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pdc.Hosting;
using Pdc.Messaging.ServiceBus;
using PlataformaPDCOnline.Editable.CommandsHandlers;
using PlataformaPDCOnline.Editable.pdcOnline.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaPDCOnline
{
    class Program
    {
        private static IConfiguration configuration;

        static void Main(string[] args)
        {
            configuration = GetConfiguration();
            var services = GetBoundedContextServices();

            Run(services);

            Console.WriteLine("press...");
            Console.ReadLine();
            
        }

        private static async void Run(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var boundedContext = services.GetRequiredService<IHostedService>();
                try
                {
                    await boundedContext.StartAsync(default);
                    Thread.Sleep(60000);
                }
                finally
                {
                    Console.WriteLine("stoping");
                    await boundedContext.StopAsync(default);
                }
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var c = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "DistributedRedisCache:InstanceName", "Cache." },
                    { "RedisDistributedLocks:InstanceName", "Locks." },
                    { "ProcessManager:Sender:EntityPath", "core-test-commands" },
                    { "BoundedContext:Publisher:EntityPath", "core-test-events" },
                    { "CommandHandler:Receiver:EntityPath", "core-test-commands" },
                    { "Denormalization:Subscribers:0:EntityPath", "core-test-events" },
                    { "Denormalization:Subscribers:0:SubscriptionName", "core-test-events-denormalizers" }
                })
                .AddUserSecrets(assembly, optional: true)
                .AddEnvironmentVariables()
                .Build();

#if !DEBUG
            return new ConfigurationBuilder()
                .AddConfiguration(c)
                .AddAzureKeyVault(c["AzureKeyVault:Uri"], c["AzureKeyVault:ClientId"], c["AzureKeyVault:ClientSecret"])
                .Build();
#else
            return c;
#endif
        }

        private static IServiceProvider GetBoundedContextServices()
        {
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddDebug());

            services.AddAzureServiceBusCommandReceiver(
                builder =>
                {
                    builder.AddCommandHandler<CreateWebUser, CreateWebUserHandler>();
                    builder.AddCommandHandler<UpdateWebUser, UpdateWebUserHandler>();
                    builder.AddCommandHandler<DeleteWebUser, DeleteWebUserHandler>();

                    builder.AddCommandHandler<CreateWebAccessGroup, CreateWebAccessGroupHandler>();
                },
                new Dictionary<string, Action<CommandBusOptions>>
                {
                    ["Core"] = options => configuration.GetSection("CommandHandler:Receiver").Bind(options)
                });

            services.AddAggregateRootFactory();
            services.AddUnitOfWork();
            services.AddRedisDistributedLocks(options => configuration.GetSection("RedisDistributedLocks").Bind(options));
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configuration["DistributedRedisCache:Configuration"];
                options.InstanceName = configuration["DistributedRedisCache:InstanceName"];
            });

            services.AddAzureServiceBusEventPublisher(options => configuration.GetSection("BoundedContext:Publisher").Bind(options));

            services.AddHostedService<HostedService>();

            return services.BuildServiceProvider();
        }
    }
}
