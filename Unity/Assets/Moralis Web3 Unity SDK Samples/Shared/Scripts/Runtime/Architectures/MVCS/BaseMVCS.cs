using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Sdk.DesignPatterns.Creational.Singleton.SingletonMonobehaviour;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.Architectures.MVCS
{
	public class BaseMVCS<U> : SingletonMonobehaviour<U> where U: MonoBehaviour
	{
		public static IDebug Debug = new UnityEngineDebug();
	}
	
    /// <summary>
    /// Replace with comments...
    /// </summary>
    public class BaseMVCS2<T> : BaseMVCS<T> where T: MonoBehaviour
    {
        //  Properties ------------------------------------

		
        //  Fields ----------------------------------------

		
        //  Initialization Methods-------------------------

		
        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
    }
}