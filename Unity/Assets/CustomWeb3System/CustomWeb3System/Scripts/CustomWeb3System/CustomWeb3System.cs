using UnityEngine;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// The scheme allows for an external scope to choose WHICH implementation of ICustomWeb3System the game uses.
    /// </summary>
    public interface ICustomWeb3SystemInstanceFactory
    {
        ICustomWeb3System Create();
    }
    
    public static class CustomWeb3System
    {
        private static ICustomWeb3SystemInstanceFactory _customWeb3SystemInstanceFactory;
        public static void SetICustomWeb3SystemInstanceFactory(ICustomWeb3SystemInstanceFactory customWeb3SystemInstanceFactory)
        {
            _customWeb3SystemInstanceFactory = customWeb3SystemInstanceFactory;
        }
        
        //
        static readonly object _padlock = new object();
        private static ICustomWeb3System _instance;
        public static ICustomWeb3System Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_customWeb3SystemInstanceFactory == null)
                    {
                        Debug.LogError("Must first set _customWeb3SystemInstanceFactory from outside this class.");
                    }
                    _instance = _customWeb3SystemInstanceFactory.Create();
                }

                return _instance;
            }
        }
    }
}