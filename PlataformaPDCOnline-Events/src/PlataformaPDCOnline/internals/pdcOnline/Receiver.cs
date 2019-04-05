using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Pdc.Hosting;
using Pdc.Integration.BoundaryContext;
using Pdc.Integration.Denormalization;
using Pdc.Messaging.ServiceBus;
using PlataformaPDCOnline.Editable.EventsHandlers;
using PlataformaPDCOnline.Editable.pdcOnline.Events;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PlataformaPDCOnline.internals.pdcOnline
{
    public class Receiver
    {
        private static Receiver receiver;

        private readonly IConfiguration configuration;
        private IHostedService boundedContext;
        private readonly IServiceProvider services;

        public static Receiver Singelton()
        {
            if (receiver == null) receiver = new Receiver();
            return receiver;
        }

        private Receiver()
        {
            configuration = GetConfiguration();
            services = GetBoundedContextServices();

            RunServices();
        }

        public IServiceProvider GetServices()
        {
            return services;
        }

        private async void RunServices()
        {
            Console.WriteLine("Servicio Iniciadondo..");
            using (services.GetRequiredService<IServiceScope>())
            {
                boundedContext = services.GetRequiredService<IHostedService>();
                
                await boundedContext.StartAsync(default);
            }
            Console.WriteLine("Iniciado.."); //muestra por consola
        }

        public async void EndServices()
        {
            using (services.GetRequiredService<IServiceScope>())
            {
                await boundedContext.StopAsync(default);
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var c = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
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

        private IServiceProvider GetBoundedContextServices()
        {
            var services = new ServiceCollection();

            //services.AddLogging(builder => builder.AddDebug());
            services.AddLogging(logginBuilder =>
            {
                logginBuilder.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                logginBuilder.AddApplicationInsights((string)Program.GetApplicationConfiguration("instrumentationKey"));
            });

            services.AddAzureServiceBusEventSubscriber(
                builder =>
                {
                    builder.AddEventHandler<WebUserCreated, WebUserCreatedHandler>();
                    builder.AddEventHandler<WebUserUpdated, WebUserUpdatedHandler>();
                    builder.AddEventHandler<WebUserDeleted, WebUserDeletedHandler>();

                    builder.AddEventHandler<WebAccessGroupCreated, WebAccessGroupCreatedHandler>();
                },
                new Dictionary<string, Action<EventBusOptions>>
                {
                    ["Core"] = options => configuration.GetSection("Denormalization:Subscribers:0").Bind(options),
                });

            services.AddHostedService<HostedService>();

            services.AddSingleton(services.BuildServiceProvider().CreateScope());

            return services.BuildServiceProvider();
        }
    }
}
