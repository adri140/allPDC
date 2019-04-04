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
            Receiver rec = new Receiver();

            Console.WriteLine("Press to Stop Services...");
            Console.ReadLine();

            Console.WriteLine("Stoping services...");
            rec.EndServices();

            Thread.Sleep(10000);
        }
    }
}
