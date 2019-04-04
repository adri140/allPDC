using PlataformaPDCOnline.internals.pdcOnline;
using System;
using System.Configuration;
using System.Threading;

namespace PlataformaPDCOnline
{
    public class Program
    {
        public static object GetApplicationConfiguration(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        static void Main(string[] args)
        {
            Receiver rec = Receiver.Singelton();

            Console.WriteLine("Press to Stop Services...");
            Console.ReadLine();

            rec.EndServices();
            Console.WriteLine("Stoping services...");

            Thread.Sleep(10000);
        }
    }
}
