using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.Reflection;

namespace PlataformaPDCOnline.Internals.applicationInsights
{
    class Telemetry //esto no me gusta, esto no se utiliza, TMP
    {
        private static Telemetry ApplicationTelemetry;

        public static Telemetry Singelton()
        {
            if (ApplicationTelemetry == null) ApplicationTelemetry = new Telemetry();
            return ApplicationTelemetry;
        }

        private static readonly string TelemetryKey = "----";

        private TelemetryClient client;

        private Telemetry()
        {
            client = GetAppTelemetryClient();
        }

        private TelemetryClient GetAppTelemetryClient()
        {
            TelemetryConfiguration.Active.DisableTelemetry = false;
            var config = new TelemetryConfiguration
            {
                InstrumentationKey = TelemetryKey,
                TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel
                {
                    DeveloperMode = Debugger.IsAttached
                }
            };

#if DEBUG
            config.TelemetryChannel.DeveloperMode = true;
#endif

            TelemetryClient client = new TelemetryClient(config);
            client.Context.Component.Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            client.Context.User.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            return client;
        }

        public void TrackEvent(string eventMessage)
        {
            client.TrackEvent(eventMessage);
        }

        public void TrackException(Exception exception)
        {
            client.TrackException(exception);
        }

        public void Flush()
        {
            client.Flush();
        }

        public void TrackMetric(MetricTelemetry metrica)
        {
            //client.TrackMetric(metrica);
            client.GetMetric(metrica.Name).TrackValue(metrica.Sum);
        }
    }
}
