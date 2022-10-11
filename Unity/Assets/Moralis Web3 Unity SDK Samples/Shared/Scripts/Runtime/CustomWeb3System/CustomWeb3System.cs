namespace MoralisUnity.Samples.Shared
{
    public class CustomWeb3System
    {
        static readonly object _padlock = new object();
        private static ICustomWeb3System _instance;
        public static ICustomWeb3System Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = MoralisOneWeb3System.Instantiate();
                    }
 
                    return _instance;
                }
            }
        }
    }
}