using UnityEngine;

namespace RMC.Shared.Components
{
	/// <summary>
	/// Repeatedly scrolls the camera background
	/// </summary>
	public class ShaderScrollerComponent : MonoBehaviour
	{
		// Properties -------------------------------------
		
		
		// Fields -----------------------------------------
		[SerializeField]
		private Renderer _renderer = null;
		
		[SerializeField]
		private Vector2 _scrollSpeed = new Vector2(0.05f, 0.005f);
		private Vector2 _mainTextureOffsetOnAwake;
		
		// Unity Methods -----------------------------------
		protected void Awake()
		{
			ScrollBy(Time.time * _scrollSpeed);
			_mainTextureOffsetOnAwake = _renderer.material.mainTextureOffset;
		}
		
		protected virtual void Update()
		{
			ScrollBy(Time.deltaTime * _scrollSpeed);
		}

		protected void OnDestroy()
		{
			// Prevents dirtying git version control on the material
			ScrollBy(_mainTextureOffsetOnAwake);
		}
		
		
		// General Methods --------------------------------
		private void ScrollBy(Vector2 scrollVector2)
		{
			if (!_renderer || scrollVector2.magnitude == 0)
			{
				return;
			}

			ScrollTo(_renderer.material.mainTextureOffset + scrollVector2);
		}
		
		private void ScrollTo(Vector2 scrollVector2)
		{
			if (!_renderer)
			{
				return;
			}
			_renderer.sharedMaterial.mainTextureOffset = scrollVector2;
		}

		private float t;
		// Event Handlers ---------------------------------
	}
}