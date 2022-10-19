using System;

namespace RMC.Shared.Exceptions
{
    /// <summary>
    /// Creates elegant exception flow for unintended Switch Defaults.
    /// </summary>
    public class SwitchDefaultException : Exception
    {
        //  Properties ------------------------------------

        
        //  Fields ----------------------------------------

        
        //  Constructor Methods ---------------------------
        public SwitchDefaultException(object value) :
            base($"Switch Default Exception for Case: '{value.ToString()}'")
        {
            
        }

        //  Methods ---------------------------------------
        public static void Throw (object value)
        {
            throw new SwitchDefaultException (value);
        }
        
        
        //  Event Handlers --------------------------------
        
        
    }
}