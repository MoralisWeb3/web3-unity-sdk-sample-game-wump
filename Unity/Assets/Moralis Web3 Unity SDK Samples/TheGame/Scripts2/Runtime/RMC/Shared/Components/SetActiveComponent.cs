using UnityEngine;

namespace RMC.Shared.Components
{
	/// <summary>
	/// Toggle the visibility at runtime. This is helpful
	/// to have a <see cref="GameObject"/> be visible in the editor
	/// but not at runtime.
	/// </summary>
	public class SetActiveComponent : MonoBehaviour
	{
		[SerializeField]
		private bool _isActiveOnAwake = true;

		protected void Awake()
		{
			gameObject.SetActive(_isActiveOnAwake);
			enabled = _isActiveOnAwake;
		}
	}
}
