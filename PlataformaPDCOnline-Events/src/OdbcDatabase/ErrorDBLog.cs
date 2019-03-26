using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OdbcDatabase
{
    public class ErrorDBLog
    {
        public static void Write(string dataException)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:/errorDB.txt", true))
                {
                    file.WriteLine("ErrorDB-EventClient " + DateTime.Now + "=> " + dataException);
                }
            }
            catch (FileNotFoundException fie)
            {
                Console.WriteLine(fie.Message);
            }
        }
    }
}
