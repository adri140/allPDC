using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaPDCOnline.Internals.excepciones
{
    class NoCompletCommandSend : Exception
    {
        public NoCompletCommandSend(String message) : base(message)
        {
        }
    }
}
