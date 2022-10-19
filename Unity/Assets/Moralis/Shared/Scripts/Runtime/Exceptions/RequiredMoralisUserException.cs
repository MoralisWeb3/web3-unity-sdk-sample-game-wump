using System;

namespace MoralisUnity.Samples.Shared.Exceptions
{
    /// <summary>
    /// Thrown when initialization is required but not complete.
    /// </summary>
    public class RequiredMoralisUserException : Exception
    {
        //  Properties ------------------------------------


        //  Fields ----------------------------------------


        //  Constructor Methods ---------------------------
        public RequiredMoralisUserException() :
            base($"Required MoralisUser. Must Authenticate. ")
        {

        }


        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------


    }
}
