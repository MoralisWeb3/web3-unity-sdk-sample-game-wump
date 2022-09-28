using MoralisUnity.Samples.Shared.Attributes;
using MoralisUnity.Samples.Shared.Data.Types.Configuration;
using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View;
using UnityEngine;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types.Configuration
{
    /// <summary>
    /// Main configuration for the game. Click the instance of this class in the project to view/edit
    /// </summary>
    [ReferenceByGuid (Guid = "259c0de8152c6974e811ad9ec6e1cb58")]
    [CreateAssetMenu( menuName = TheGameConstants.PathCreateAssetMenu + "/" + Title,  fileName = Title)]
    public class TheGameConfiguration : BaseConfiguration<TheGameConfiguration>
    {
        // Properties -------------------------------------
        public TheGameView TheGameViewPrefab { get { return _theGameViewPrefab; } }
        public SceneData IntroSceneData { get { return _sceneDataStorage.SceneDatas[0];}}
        public SceneData AuthenticationSceneData { get { return _sceneDataStorage.SceneDatas[1];}}
        public SceneData SettingsSceneData { get { return _sceneDataStorage.SceneDatas[2];}}
        public SceneData DeveloperConsoleSceneData { get { return _sceneDataStorage.SceneDatas[3];}}
        public SceneData ViewCollectionSceneData { get { return _sceneDataStorage.SceneDatas[4];}}
        public SceneData GameSceneData { get { return _sceneDataStorage.SceneDatas[5]; } }

        public TheGameServiceType TheGameServiceType { get { return _theGameServiceType;}}
        

        // Fields -----------------------------------------
        public const string Title = "TheGameConfiguration";
        
        [Header("References (Project)")]
        [SerializeField]
        private TheGameView _theGameViewPrefab = null;

        [SerializeField]
        private SceneDataStorage _sceneDataStorage = null;
  
        [Header("Settings (Edit-Time Only)")]
        
        [Tooltip("Use either Moralis Database (dev) or Moralis Web3 (prod)")]
        [SerializeField]
        public TheGameServiceType _theGameServiceType = TheGameServiceType.Null;
        
        // Unity Methods ----------------------------------
        protected override void OnValidate()
        {
            base.OnValidate();
        }


        // General Methods --------------------------------

		
        // Event Handlers ---------------------------------
    }
}