
#if UNITY_EDITOR
using ParrelSync;
#else
using System;
#endif //UNITY_EDITOR

namespace RMC.Shared
{
	//  Namespace Properties ------------------------------


	//  Class Attributes ----------------------------------


	/// <summary>
	/// Wrap the complexity of "#if" here for convenience
	/// </summary>
	public static class ClonesManagerWrapper 
	{
		//  Events ----------------------------------------


		//  Properties ------------------------------------
		public static bool HasClonesManager
		{
			get
			{
#if UNITY_EDITOR
				return ClonesManager.GetArgument() != null;
#else
				return false;
#endif //UNITY_EDITOR
			}
		}
		
		public static string GetArgument
		{
			get
			{
#if UNITY_EDITOR
				return ClonesManager.GetArgument();
#else
				throw new Exception ("Cannot GetArgument. Must first check HasClonesManage==true");
#endif //UNITY_EDITOR
			}
		}
		
		public static bool IsClone
		{
			get
			{
#if UNITY_EDITOR
				return ClonesManager.IsClone();
#else
				throw new Exception ("Cannot IsClone. Must first check HasClonesManage==true");
#endif //UNITY_EDITOR
			}
		}

		//  Fields ----------------------------------------
	
		
		//  Methods ---------------------------------------

		
		//  Event Handlers --------------------------------

	}
}
