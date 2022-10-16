using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Sdk.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MoralisUnity.Samples.Shared.Components
{
	public class SceneManagerComponentUnityEvent : UnityEvent<SceneManagerComponent, string, string> {}
	
	/// <summary>
	/// Determines "was the active scene loaded directly?
	/// * (true) Loaded directly
	/// * (false) Loaded indirectly at runtime
	/// </summary>
	public class SceneManagerComponent : MonoBehaviour , IInitializable
	{
		// Events -----------------------------------------
		public SceneManagerComponentUnityEvent OnSceneLoadingEvent = new SceneManagerComponentUnityEvent();
		public SceneManagerComponentUnityEvent OnSceneLoadedEvent = new SceneManagerComponentUnityEvent();
		public bool IsInitialized { get; private set; }
		
		// Properties -------------------------------------

		// Fields -----------------------------------------
		private static string _sceneNameLoadedDirectly = "";
		private static string _sceneNamePrevious = "";
		private SceneTransition _sceneTransition;
		private ImageAndCanvasView _imageAndCanvasView;

		// Unity Methods ----------------------------------
		protected void Awake ()
		{
			_sceneNameLoadedDirectly = SceneManager.GetActiveScene().name;
		}
		
		protected void Start ()
		{
			SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
			SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
			Debug.Log("Start!!!!!");
		}

		
		public void Initialize(SceneTransition sceneTransition, ImageAndCanvasView imageAndCanvasView)
		{
			if (IsInitialized) return;
			IsInitialized = true;
			_sceneTransition = sceneTransition;
			_imageAndCanvasView = imageAndCanvasView;
		}
		
		public void Initialize()
		{
			//Must call the other Initialize();
		}

		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}

		// General Methods --------------------------------
		public bool WasActiveSceneLoadedDirectly()
		{
			return _sceneNameLoadedDirectly == SceneManager.GetActiveScene().name;
		}
		
		public void LoadScenePrevious()
		{
			RequireIsInitialized();
			LoadScene(_sceneNamePrevious);
		}
		
		public async void LoadScene(string sceneName)
		{
			RequireIsInitialized();
			if (string.IsNullOrEmpty(sceneName))
			{
				Custom.Debug.LogWarning($"Cannot LoadScene() when sceneName={sceneName}. That is ok.");
				return;
			}

			await _sceneTransition.ApplyTransition(_imageAndCanvasView, () =>
			{
				_sceneNamePrevious = SceneManager.GetActiveScene().name;
				
				//Debug.LogWarning($" 1 _sceneNamePrevious={_sceneNamePrevious} sceneName={sceneName} ");
				OnSceneLoadingEvent.Invoke(this, _sceneNamePrevious, sceneName);
				SceneManager.LoadScene(sceneName);

			});
		}
		
		// Event Handlers ---------------------------------
		private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			//Debug.LogWarning($"================================= 1 _sceneNamePrevious={_sceneNamePrevious} scene.name={scene.name} ");
			OnSceneLoadedEvent.Invoke(this, _sceneNamePrevious, scene.name);
		}

	}
}
