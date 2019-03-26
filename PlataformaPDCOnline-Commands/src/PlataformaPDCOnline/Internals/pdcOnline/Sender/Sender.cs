using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pdc.Hosting;
using Pdc.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace PlataformaPDCOnline.Internals.pdcOnline.Sender
{
    //contiene todos los datos para poder crear el sender, usamos singelton para que solo se pueda instanciar una vez
    public class Sender
    {

        private static Sender CommandsSender;

        public static Sender Singelton()
        {
            if (CommandsSender == null) CommandsSender = new Sender();
            return CommandsSender;
        }

        private readonly IConfiguration configuration;
        private readonly IServiceProvider services;
        private IHostedService boundedContext;
        private ICommandSender sender;

        private Sender()
        {
            configuration = GetConfiguration();

            services = GetBoundedContextServices();

            InicializeAsync();
        }

        private async void InicializeAsync()
        {
            using (services.GetRequiredService<IServiceScope>())
            {
                boundedContext = services.GetRequiredService<IHostedService>();

                await boundedContext.StartAsync(default); //iniciamos todos los servicios

                sender = services.GetRequiredService<ICommandSender>();
            }
        }

        public async Task SendCommand(Command command)
        {
            if (command != null)
            {
                using (services.GetRequiredService<IServiceScope>())
                {
                    await sender.SendAsync(command);
                    //Console.WriteLine("enviando command");
                }
            }
        }

        public async Task EndJobAsync()
        {
            using (services.GetRequiredService<IServiceScope>())
            {
                if (boundedContext != null)
                {
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
                    { "ProcessManager:Sender:EntityPath", "core-test-commands" }
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

            services.AddLogging(builder => builder.AddDebug()); 
            
            services.AddAzureServiceBusCommandSender(options => configuration.GetSection("ProcessManager:Sender").Bind(options));
            
            services.AddHostedService<HostedService>();

            services.AddSingleton(scope => services.BuildServiceProvider().CreateScope());
            
            services.AddSingleton(appIns => GetAppTelemetryClient());

            return services.BuildServiceProvider();
        }

        private TelemetryClient GetAppTelemetryClient()
        {
            TelemetryConfiguration.Active.DisableTelemetry = false;

            var config = new TelemetryConfiguration()
            {
                InstrumentationKey = "----"
            };

            config.TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel
            {
                DeveloperMode = Debugger.IsAttached
            };

#if DEBUG
            config.TelemetryChannel.DeveloperMode = true;
#endif

            TelemetryClient client = new TelemetryClient(config);
            client.Context.Component.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            //client.Context.User.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            return client;
        }

        public void TrackException(Exception e)
        {
            services.GetRequiredService<TelemetryClient>().TrackException(e);
        }

        public void TrackEvent(string evente)
        {
            services.GetRequiredService<TelemetryClient>().TrackEvent(evente);
        }

        public void TrackTrace(string trace)
        {
            services.GetRequiredService<TelemetryClient>().TrackTrace(trace);
        }

        public void TrackMetric(MetricTelemetry metric)
        {
            services.GetRequiredService<TelemetryClient>().TrackMetric(metric);
        }

        public void Flush()
        {
            services.GetRequiredService<TelemetryClient>().Flush();
        }
    }
}
