
using MoralisUnity.Samples.Shared;

namespace MoralisUnity.Samples.TheGame
{
    /// <summary>
    /// Helper Values
    /// </summary>
    public static class TheGameConstants
    {
        // Fields -----------------------------------------
        public const string ProjectName = "Web3 Magic Treasure Chest";
        public const string PathCreateAssetMenu = MyMoralisWrapper.PathMoralisSamplesCreateAssetMenu + "/" + ProjectName;
        
        ///////////////////////////////////////////
        // MenuItem Path
        ///////////////////////////////////////////
        public const string Moralis = "Moralis";
        public const string OpenReadMe = "Open ReadMe";
        private const string PathMoralisCreateAssetMenu = Moralis + "/" + MyMoralisWrapper.Web3UnitySDK;
        private const string PathMoralisWindowMenu = "Window/" + Moralis + "/" + MyMoralisWrapper.Web3UnitySDK;

        ///////////////////////////////////////////
        // MenuItem Priority
        ///////////////////////////////////////////

        // Skipping ">10" shows a horizontal divider line.
        public const int PriorityMoralisWindow_Primary = 10;
        public const int PriorityMoralisWindow_Secondary = 100;
        public const int PriorityMoralisWindow_Examples = 1000;
        public const int PriorityMoralisWindow_Samples = 10000;

        ///////////////////////////////////////////
        // Display Text
        ///////////////////////////////////////////
        public const string Registering = "Registering Player";
        public const string Unregistering = "Unregistering Player";
        public const string SafeReregistering = "Safely Unregistering & Registering";
        public const string GettingPrizes = "Getting Prizes";
        public const string GettingTransferLogHistory = "Getting Transfer Log History";
        public const string TransferingGold = "Transfering Gold";
        public const string TransferingPrize = "Transfering Prize";
        public const string MustBeRegistered = "Must Be Registered";
        public const string MustNotBeRegistered = "Must Not Be Registered";
        //
        public const uint GoldOnTransfer = 25;
        public const uint PrizesOnTransfer = 1;
        //
        public const uint GoldMax = 100; //May be a ui-only limit
        public const uint PrizesMax = 6; //May be a ui-only limit
    }
}