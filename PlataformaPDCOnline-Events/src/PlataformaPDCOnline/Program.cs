using PlataformaPDCOnline.internals.pdcOnline;
using System;
using System.Threading;

namespace PlataformaPDCOnline
{
    class Program
    {
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
