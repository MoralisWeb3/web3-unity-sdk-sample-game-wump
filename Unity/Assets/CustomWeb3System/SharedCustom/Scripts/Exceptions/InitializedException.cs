using System;

namespace MoralisUnity.Samples.CustomShared.Exceptions
{
    /// <summary>
    /// Thrown when initialization is required NOT to happen yet, but has happened.
    /// </summary>
    public class InitializedException : Exception
    {
        //  Properties ------------------------------------

        //  Fields ----------------------------------------

        //  Constructor Methods ---------------------------
        public InitializedException(object obj) :
            base($"Initialized Exception() class = '{obj.GetType().Name}'")
        {
        }

        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------
    }
}
