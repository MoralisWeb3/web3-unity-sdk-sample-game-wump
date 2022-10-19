namespace MoralisUnity.Samples.Shared.Debugging
{
    /// <summary>
    /// Define SOME (not yet all, feel free to add more) of the methods supported by
    /// UnityEngine.Debug. 
    /// </summary>
    public interface IDebug
    {
        // Properties -------------------------------------
        bool IsEnabled { get; set; }
        
        // General Methods --------------------------------
        void Log(object message);
        void Log(object message, UnityEngine.Object context);
        void LogError(object message);
        void LogError(object message, UnityEngine.Object context);
        void LogException(System.Exception exception);
        void LogException(System.Exception exception, UnityEngine.Object context);
        void LogWarning(object message);
        void LogWarning(object message, UnityEngine.Object context);
    }
}

namespace MoralisUnity.Samples.Shared.Debugging
{
    /// <summary>
    /// Added so that <see cref="BaseConfiguration{T}"/> can toggle IsDebugLog for the project
    /// </summary>
    public class UnityEngineDebug : IDebug
    {
        // Properties -------------------------------------
        public bool IsEnabled { get { return _isEnabled; } set { _isEnabled = value; } }
        
        // Fields -----------------------------------------
        //Must be static
        private static bool _isEnabled = true;
        
        // General Methods --------------------------------
        public void Log( object message )
        {
            // Did you click the console and arrive here?
            // Sorry, this is the custom logger. Click on a lower result in the callstack
            // TODO: Compile this class to a dll. That will prevent clicks from opening exactly here
            if (!IsEnabled) return;
            UnityEngine.Debug.Log(message);
        }
        
        public void Log( object message, UnityEngine.Object context )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.Log(message, context);
        }
        
        
        public void LogError( object message )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.LogError(message);
        }
        
        
        public void LogError( object message, UnityEngine.Object context )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.LogError(message, context);
        }
        
        
        public void LogException( System.Exception exception )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.LogException(exception);
        }
        
        
        public void LogException( System.Exception exception, UnityEngine.Object context )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.LogException(exception, context);
        }
        
        
        public void LogWarning( object message )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.LogWarning(message);
        }
        
        
        public void LogWarning( object message, UnityEngine.Object context )
        {
            if (!IsEnabled) return;
            UnityEngine.Debug.LogWarning(message, context);
        }
        // Event Handlers ---------------------------------
        
    }
}


namespace MoralisUnity.Samples.Shared.Debugging
{
    /// <summary>
    /// Added so that <see cref="BaseConfiguration"/> can toggle IsDebugLog for the project
    ///
    /// USAGE
    ///     * Debug.Log("Hello"); // to use unity's logging
    ///     * Shared.Debug.Log("Hello"); // to use a custom wrapper (with IsEnabled toggle) for unity's logging
    /// 
    /// </summary>
    public static class Custom //Keep naming. While a bit odd, I like the resulting format of "Shared.Debug.Log()" from any context
    {
        // Properties -------------------------------------
        public static IDebug Debug = new UnityEngineDebug();
        
        // Fields -----------------------------------------
        
        
        // General Methods --------------------------------
        
        
        // Event Handlers ---------------------------------
        
    }

}



 




