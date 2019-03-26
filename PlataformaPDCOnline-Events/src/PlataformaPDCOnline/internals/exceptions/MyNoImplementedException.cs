using System;

namespace PlataformaPDCOnline.internals.exceptions
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
