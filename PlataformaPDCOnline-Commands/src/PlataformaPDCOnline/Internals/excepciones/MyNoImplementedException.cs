using System;

namespace PlataformaPDCOnline.Internals.excepciones
{
    public class MyNoImplementedException : Exception
    {
        public MyNoImplementedException()
        {
        }

        public MyNoImplementedException(string message)
            : base(message)
        {
        }
    }
}
