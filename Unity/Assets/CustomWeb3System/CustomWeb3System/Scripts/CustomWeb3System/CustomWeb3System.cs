namespace MoralisUnity.Samples.Shared
{
    public static class CustomWeb3System
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
                        // Optional: Manually update this one-liner to change implementation
                        _instance = MoralisTwoWeb3System.Instantiate();
                        _instance.CustomWeb3WalletSystem = new WalletConnectWeb3WalletSystem();
                        _instance.CustomBackendSystem = new PlayFabBackendSystem();
                    }
 
                    return _instance;
                }
            }
        }
    }
}