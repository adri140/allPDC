using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Pdc.Hosting;
using Pdc.Messaging;
using System;
using System.Collections.Generic;
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
        private readonly IServiceProvider Services;
        private IHostedService boundedContext;
        private ICommandSender sender;

        private Sender()
        {
            configuration = GetConfiguration();

            Services = GetBoundedContextServices();

            //services.GetRequiredService<ILogger<Sender>>().LogInformation("hey, el logger esta trabajando, Un saludo");
            //esto supuestamente funciona
            //https://docs.microsoft.com/bs-latn-ba/azure////azure-monitor/app/ilogger 

            InicializeAsync();
        }

        public IServiceProvider GetServices()
        {
            return Services;
        }

        private async void InicializeAsync()
        {
            using (Services.GetRequiredService<IServiceScope>())
            {
                boundedContext = Services.GetRequiredService<IHostedService>();

                await boundedContext.StartAsync(default); //iniciamos todos los servicios

                sender = Services.GetRequiredService<ICommandSender>();
            }
        }

        public Boolean SendCommand(Command command)
        {
            if (command != null)
            {
                using (Services.GetRequiredService<IServiceScope>())
                {
                    Task t = sender.SendAsync(command);
                    t.Wait();
                    return true;
                }
            }
            return false;
        }

        public async Task EndJobAsync()
        {
            using (Services.GetRequiredService<IServiceScope>())
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


            var channel = new InMemoryChannel();
            services.Configure<TelemetryConfiguration>(
                (config) =>
                {
                    config.TelemetryChannel = channel;
                }    
            );

            services.AddLogging(logginBuilder =>
            {
                logginBuilder.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                logginBuilder.AddApplicationInsights("--Instrumentation-Key--");
            });
            //services.AddLogging(builder => builder.AddDebug());
            
            services.AddAzureServiceBusCommandSender(options => configuration.GetSection("ProcessManager:Sender").Bind(options));
            
            services.AddHostedService<HostedService>();

            services.AddSingleton(scope => services.BuildServiceProvider().CreateScope());

            return services.BuildServiceProvider();
        }
    }
}
