using OdbcDatabase.excepciones;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;

namespace OdbcDatabase.database
{
    public class DatabaseTools
    {
        public static string GetConnectionString(string section)
        {
            string connectionString = "";

            try
            {
                string[] linias = System.IO.File.ReadAllLines(@"C:/connectionDatabase.ini");

                Boolean lectura = false;

                foreach (string linia in linias)
                {
                    if (lectura && linia.Trim().Length > 0) connectionString = connectionString + linia.Trim() + ";";
                    else lectura = linia.Equals(section) ? true : false;
                }
            }
            catch(FileNotFoundException notFound)
            {
                Console.WriteLine(notFound.Message);
            }

            return connectionString;
        }

        /*
         * quita la ultima coincidencia de un String
         */
        public static string GetStringPrepared(string aPreparar, string regex)
        {
            return aPreparar.ToString().Substring(0, aPreparar.ToString().LastIndexOf(regex)).Trim();
        }

        public static void InsertParameters(Dictionary<string, object> dataParameter, Dictionary<string, OdbcType> typeData, OdbcCommand command)
        {
            foreach (string key in dataParameter.Keys)
            {
                try
                {
                    if (dataParameter.GetValueOrDefault(key) != null) command.Parameters.Add("@" + key, typeData.GetValueOrDefault(key)).Value = dataParameter.GetValueOrDefault(key);
                }
                catch(Exception e)
                {
                    throw new MyOdbcException(e.Message);      
                }
            }
        }
    }
}
