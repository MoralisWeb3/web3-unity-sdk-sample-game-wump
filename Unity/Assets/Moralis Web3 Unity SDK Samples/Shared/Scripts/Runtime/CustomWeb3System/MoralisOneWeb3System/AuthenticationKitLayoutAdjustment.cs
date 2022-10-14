using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class AuthenticationKitLayoutAdjustment: MonoBehaviour
	{
		// Properties -------------------------------------
		[SerializeField]
		private RectTransform _platformsRectTransform;
	
		[SerializeField]
		private RectTransform _textsRectTransform;

		[SerializeField]
		private RectTransform _ButtonsRectTransform;

		// Properties -------------------------------------
		
		// Fields -----------------------------------------

		// Initialization Methods -------------------------
		protected void Start()
		{
			return;
			
			LogExistingValues();
			Debug.Log(_platformsRectTransform.offsetMin);
			Debug.Log(_platformsRectTransform.offsetMax);
			
			// Move Layout Upwards to allow for custom "Cancel" Button
			//_platformsRectTransform.offsetMin = new Vector2(0, 50);
		}


		// General Methods --------------------------------
		protected void LogExistingValues()
		{
			return;

			Debug.Log($"_p offsetMin:{_platformsRectTransform.offsetMin}");
			Debug.Log($"_p offsetMax:{_platformsRectTransform.offsetMax}");
			Debug.Log($"_p offsetMin:{_platformsRectTransform.offsetMin}");
			Debug.Log($"_p offsetMin:{_platformsRectTransform.offsetMin}");
			Debug.Log($"_p offsetMax:{_platformsRectTransform.offsetMax}");
			Debug.Log($"_p offsetMax:{_platformsRectTransform.offsetMax}");
			// Move Layout Upwards to allow for custom "Cancel" Button
			//_platformsRectTransform.offsetMin = new Vector2(0, 50);
		}
		
		// Event Handlers ---------------------------------
	
	}
}