using System;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.Exceptions
{
    /// <summary>
    /// Thrown when isRegistered is required true but is not true.
    /// </summary>
    public class RequiredIsRegisteredException : Exception
    {
        //  Properties ------------------------------------


        //  Fields ----------------------------------------


        //  Constructor Methods ---------------------------
        public RequiredIsRegisteredException() :
            base($"IsRegistered Must Be True. Must Register. ")
        {

        }


        //  Methods ---------------------------------------


        //  Event Handlers --------------------------------


    }
}
