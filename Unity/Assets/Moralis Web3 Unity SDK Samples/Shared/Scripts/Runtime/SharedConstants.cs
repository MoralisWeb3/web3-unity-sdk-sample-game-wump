using MoralisUnity.Sdk.Constants;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// Helper values
    /// </summary>
    public static class SharedConstants
    {
        //  Fields  -----------------------------------------------
        
        ///////////////////////////////////////////
        // MenuItem Path
        ///////////////////////////////////////////
        public const string Web3UnitySDKExamples = "Web3 Unity SDK Examples";
        public const string OpenReadMe = MoralisConstants.Open + " " + "ReadMe";
        public const string PathMoralisSamplesCreateAssetMenu = Moralis + " " + MoralisConstants.Web3UnitySDK + "/Samples/" + Web3UnitySDKExamples + "/";
        public const string PathMoralisSharedCreateAssetMenu = Moralis + " " + MoralisConstants.Web3UnitySDK + "/Shared/" + Web3UnitySDKExamples + "/";
        public const string PathMoralisSamplesAssetsMenu = "Assets/Moralis Web3 Unity SDK/Samples";
        
        ///////////////////////////////////////////
        // MenuItem Priority
        ///////////////////////////////////////////

        // Skipping ">10" shows a horizontal divider line.
        public const int PriorityMoralisTools_Primary = 10;
        public const int PriorityMoralisTools_Secondary = 100;
        public const int PriorityMoralisTools_Examples = 1000;
        public const int PriorityMoralisTools_Examples_Sub = 5000;
        public const int PriorityMoralisTools_Samples = 10000;
        public const int PriorityMoralisAssets_Examples = 1;
        
        ///////////////////////////////////////////
        // Display Text
        ///////////////////////////////////////////
        public const string Moralis = "Moralis";
       
        public const string Web3UnitySDK = "Web3 Unity SDK";
        public const string Web3UnitySDKVersion = "v1.2.4"; // This may be out of date. Check to Manifest.json, then re-update here
        public const string ProductWithVersion = SharedConstants.Moralis + SharedConstants.Web3UnitySDK + " " + SharedConstants.Web3UnitySDKVersion;
        
        // Display Text
        public const string Authenticate = "Authenticate";
        public const string Logout = "Logout";
        public static string WalletConnectNullReferenceException = $"WalletConnect.Instance must not be null. Add the WalletConnect.prefab to your scene.";
        
    }
}