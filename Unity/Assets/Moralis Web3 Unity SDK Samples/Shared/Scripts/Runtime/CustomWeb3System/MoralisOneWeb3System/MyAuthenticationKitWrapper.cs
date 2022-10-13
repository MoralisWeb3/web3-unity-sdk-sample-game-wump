using MoralisUnity.Kits.AuthenticationKit;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Shared
{
	/// <summary>
	/// Custom wrapper for the client-side of Moralis functionality
	/// </summary>
	public class MyAuthenticationKitWrapper : MonoBehaviour
	{
		// Properties -------------------------------------
		[HideInInspector]
		public UnityEvent OnConnected = new UnityEvent();
		
		[HideInInspector]
		public UnityEvent OnDisconnected = new UnityEvent();
	
		// Properties -------------------------------------
		
		// Fields -----------------------------------------
		[SerializeField] 
		private AuthenticationKit _authenticationKit;

		// Initialization Methods -------------------------
		protected void Start()
		{
			//Forward the event execution WITHOUT REQUIRING that consuming scope has dependency on Moralis code
			_authenticationKit.OnConnected.AddListener(() =>
			{
				OnConnected.Invoke();
			});
			
			_authenticationKit.OnDisconnected.AddListener(() =>
			{
				OnDisconnected.Invoke();
			});
		}


		// General Methods --------------------------------
		
		
		// Event Handlers ---------------------------------
	
	}
}