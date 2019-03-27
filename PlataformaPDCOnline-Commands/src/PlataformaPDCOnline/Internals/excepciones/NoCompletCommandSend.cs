using System;

namespace PlataformaPDCOnline.Internals.excepciones
{
    class NoCompletCommandSend : Exception
    {
        public NoCompletCommandSend(String message) : base(message)
        {
        }
    }
}
