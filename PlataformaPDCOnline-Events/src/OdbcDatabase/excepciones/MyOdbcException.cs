using System;

namespace OdbcDatabase.excepciones
{
    public class MyOdbcException : Exception
    {
        public MyOdbcException()
        {
        }

        public MyOdbcException(string message)
            : base(message)
        {
        }
    }
}
