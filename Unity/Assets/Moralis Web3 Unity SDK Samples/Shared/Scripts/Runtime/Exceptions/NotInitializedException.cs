using System;

namespace MoralisUnity.Samples.Shared.Exceptions
{
    /// <summary>
    /// Thrown when initialization is required but not complete.
    /// </summary>
    public class NotInitializedException : Exception
    {
        //  Properties ------------------------------------


        //  Fields ----------------------------------------


        //  Constructor Methods ---------------------------
        public NotInitializedException(object obj) :
            base($"Not Initialized Exception() class = '{obj.GetType().Name}'")
        {

        }


        //  Methods ---------------------------------------



        //  Event Handlers --------------------------------


    }
}
