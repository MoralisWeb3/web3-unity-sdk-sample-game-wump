using System;

namespace MoralisUnity.Samples.SharedCustom.Exceptions
{
    /// <summary>
    /// Thrown when initialization is required but not complete.
    /// </summary>
    public class NotAuthenticatedException : Exception
    {
        //  Properties ------------------------------------

        //  Fields ----------------------------------------

        //  Constructor Methods ---------------------------
        public NotAuthenticatedException(object obj) :
            base($"Not Authenticated Exception() class = '{obj.GetType().Name}'")
        {
        }

        //  Methods ---------------------------------------

        //  Event Handlers --------------------------------
    }
}
