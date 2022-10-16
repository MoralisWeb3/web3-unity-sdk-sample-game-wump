using MoralisUnity.Samples.Shared.Attributes;
using MoralisUnity.Samples.Shared.Data.Types.Configuration;
using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService;
using MoralisUnity.Samples.TheGame.MVCS.View;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.Configuration
{
    /// <summary>
    /// Main configuration for the game. Click the instance of this class in the project to view/edit
    /// </summary>
    [ReferenceByGuid (Guid = "259c0de8152c6974e811ad9ec6e1cb58")]
    [CreateAssetMenu( menuName = TheGameConstants.PathCreateAssetMenu + "/" + Title,  fileName = Title)]
    public class TheGameConfiguration : BaseConfiguration<TheGameConfiguration>
    {
        
        // Properties -------------------------------------
        public SceneTransition SceneTransition { get { return _sceneTransition; } }
        
        public TheGameView TheGameViewPrefab { get { return _theGameViewPrefab; } }
        public SceneData IntroSceneData { get { return _sceneDataStorage.SceneDatas[0];}}
        public SceneData AuthenticationSceneData { get { return _sceneDataStorage.SceneDatas[1];}}
        public SceneData SettingsSceneData { get { return _sceneDataStorage.SceneDatas[2];}}
        public SceneData DeveloperConsoleSceneData { get { return _sceneDataStorage.SceneDatas[3];}}
        public SceneData GameSceneData { get { return _sceneDataStorage.SceneDatas[4]; } }

        public TheGameServiceType TheGameServiceType { get { return _theGameServiceType;}}
        public MultiplayerSetupServiceType MultiplayerSetupServiceType { get { return _multiplayerSetupServiceType;}}
        public bool MultiplayerIsAutoStart { get { return _multiplayerIsAutoStart;}}
        public bool MultiplayerIsGuiVisible { get { return _multiplayerIsGuiVisible;}}
        
        public UnityTransport.SimulatorParameters LanSimulatorParameters { get { return _lanSimulatorParameters;}}
            
        public TransferDialogView TransferDialogViewPrefab { get { return _transferDialogViewPrefab; } }
        
        public NetworkManagerView NetworkManagerViewPrefab { get { return _networkManagerViewPrefab; } }
        public bool IsControllingWc { get { return _isControllingWC;} }
        public string UniquePlayerPrefsSuffix { get { return _uniquePlayerPrefsSuffix; } }

        // Fields -----------------------------------------
        public const string Title = "TheGameConfiguration";

        [Tooltip("Default = ''. Optional, use for BUILDS, so each BUILD is treated unique per multiplayer/web3.")]
        [SerializeField]
        private string _uniquePlayerPrefsSuffix = "";
        
        [Header("Settings (Web3)")]
        [Tooltip("AuthKit and WalletConnect are not robust singletons. Default = true, to fix bugs.")]
        [SerializeField]
        private bool _isControllingWC = true;

        [Header("Cosmetics")]
        [SerializeField]
        private SceneTransition _sceneTransition = null;

        
        [Header("References (Project, General)")]
        [Tooltip("Holds a reference to all the scene assets. ")]
        [SerializeField]
        private SceneDataStorage _sceneDataStorage = null;

        [Tooltip("The main game view that holds the 'you are logging in...'")]
        [SerializeField]
        private TheGameView _theGameViewPrefab = null;

        [Tooltip("The ui fro 'do you want to transfer to other user'?")]
        [SerializeField] 
        private TransferDialogView _transferDialogViewPrefab;
        
        [Header("References (Project, Networking)")]
        
        [Tooltip("Holds the core, unity-specific multiplayer functionality")]
        [SerializeField] 
        private NetworkManagerView _networkManagerViewPrefab;
        
        [Header("Settings (Edit-Time Only, General)")]
        [Tooltip("Use either Moralis Database (dev) or Moralis Web3 (prod)")]
        [SerializeField]
        public TheGameServiceType _theGameServiceType = TheGameServiceType.Null;
        
        [Header("Settings (Edit-Time Only, Networking)")]
        [Tooltip("Choose. Lan = local & fast. Full = online & robust")]
        [SerializeField] 
        private MultiplayerSetupServiceType _multiplayerSetupServiceType = MultiplayerSetupServiceType.Null;

        [Tooltip("Determines if the multiplayer server will automatically start in game scene. Set to true for production.")]
        [SerializeField] 
        private bool _multiplayerIsAutoStart = true;

        [Tooltip("Determines if the multiplayer connection GUI is shown in game scene. Set to false for production.")]
        [SerializeField] 
        private bool _multiplayerIsGuiVisible = false;
        
            
        /// <summary>
        /// Can be used to simulate poor network conditions such as:
        /// - packet delay/latency
        /// - packet jitter (variances in latency, see: https://en.wikipedia.org/wiki/Jitter)
        /// - packet drop rate (packet loss)
        /// </summary>
        [SerializeField]
        [Tooltip("Optional. Can be used to simulate poor network conditions. Zeros = No lag.")]
        private UnityTransport.SimulatorParameters _lanSimulatorParameters = new UnityTransport.SimulatorParameters
        {
            PacketDelayMS = 0,
            PacketJitterMS = 0,
            PacketDropRate = 0
        };
        
        // Unity Methods ----------------------------------
        protected override void OnValidate()
        {
            base.OnValidate();
        }


        // General Methods --------------------------------

		
        // Event Handlers ---------------------------------
    }
}