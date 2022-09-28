using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.Architectures.MVCS;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Controller;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types.Configuration;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Service;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View;
using UnityEngine;

#pragma warning disable 1998
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS
{

	/// <summary>
	/// The main entry point for the whole game
	/// <para />
	/// This is a 'light' implementation of MVCS.
	/// <list type="bullet">
	///		<item>M - <see cref="TheGameModel"/> - Stores data for the game</item>
	///		<item>V - <see cref="TheGameView"/>  - Handles the UI for the game</item>
	///		<item>C - <see cref="TheGameController"/> - Handles the core functionality of the game</item>
	///		<item>S - <see cref="TheGameService"/> - Handles communication with external sources (e.g. database/servers/contracts)</item>
	/// </list>
	/// </summary>
	public class TheGameSingleton : BaseMVCS<TheGameSingleton>
	{

		// Properties -------------------------------------
		private TheGameModel TheGameModel  { get { return _theGameModel; }}
		private TheGameView TheGameView  { get { return _theGameView; }}
		public TheGameController TheGameController  { get { return _theGameController; }}
		private ITheGameService TheGameService  { get { return _theGameService; }}
		
		
		// Fields -----------------------------------------
		private TheGameModel _theGameModel;
		private TheGameView _theGameView;
		private TheGameController _theGameController;
		private ITheGameService _theGameService;
		
		// Initialization Methods -------------------------
		public override void InstantiateCompleted()
		{
			//Set screen size for Standalone
#if UNITY_STANDALONE
			Screen.SetResolution(675, 1000, false);
			Screen.fullScreen = false;
#endif

			// Name it
			gameObject.name = GetType().Name;
			
			// Model
			_theGameModel = new TheGameModel();

			// View
			TheGameView prefab = TheGameConfiguration.Instance.TheGameViewPrefab;
			_theGameView = TheGameHelper.InstantiatePrefab(prefab, transform, new Vector3(0, 0, 0));
			
			// Service
			TheGameServiceType theGameServiceType = 
				TheGameConfiguration.Instance.TheGameServiceType;
			
			_theGameService = new TheGameServiceFactory().
				Create(theGameServiceType);

			// Controller
			_theGameController = new TheGameController(
				_theGameModel, 
				_theGameView,
				_theGameService);
			
		}

		
		// General Methods --------------------------------
		public bool WasActiveSceneLoadedDirectly()
		{
			return _theGameView.SceneManagerComponent.WasActiveSceneLoadedDirectly();
		}
		
		// Event Handlers ---------------------------------



	}
}
