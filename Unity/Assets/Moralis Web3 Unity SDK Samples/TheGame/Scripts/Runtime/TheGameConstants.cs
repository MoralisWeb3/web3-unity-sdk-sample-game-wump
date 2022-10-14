using System;
using MoralisUnity.Samples.Shared;
using RMC.Shared;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame
{
    /// <summary>
    /// Helper Values
    /// </summary>
    public static class TheGameConstants
    {
        // Fields -----------------------------------------
        public const string ProjectName = "Web3 Unity Multiplayer Playerground";
        public const string ProjectNameShort = "WUMP";
        public const string PathCreateAssetMenu = CustomWeb3SystemConstants.PathMoralisSamplesCreateAssetMenu + "/" + ProjectName;
        
        ///////////////////////////////////////////
        // Build Configuration
        ///////////////////////////////////////////
        public static Vector2 ScreenResolution = new Vector2(1600*.6f, 1000*.6f);
            
            
        ///////////////////////////////////////////
        // MenuItem Path
        ///////////////////////////////////////////
        public const string Moralis = "Moralis";
        public const string OpenReadMe = "Open ReadMe";
        private const string PathMoralisCreateAssetMenu = Moralis + "/" + CustomWeb3SystemConstants.Web3UnitySDK;
        private const string PathMoralisWindowMenu = "Window/" + Moralis + "/" + CustomWeb3SystemConstants.Web3UnitySDK;

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
        public const string MustBeAuthenticated = "Must Be Authenticated";
        public const string MustNotBeRegistered = "Must Not Be Registered";
        public const string MultiplayerConnecting = Multiplayer + " Connecting";
        public const string MultiplayerConnected = Multiplayer + " Connected";
        public const string Multiplayer = "Multiplayer";
        
        //
        public const string NetworkTransformParentMustBeNull =
            "NetworkObjects *manually placed* in scene must be on the root.";

        
        //
        public const uint GoldOnTransfer = 25;
        public const uint PrizesOnTransfer = 1;
        //
        public const uint GoldMax = 100; //May be a ui-only limit
        public const uint PrizesMax = 6; //May be a ui-only limit

        /// <summary>
        /// For FULL multiplayer mode. A unique value is treated as a unique player 
        /// </summary>
        /// <returns></returns>
        public static string GetNewUniqueTestingProfile()
        {
            return $"{Application.productName}_{Guid.NewGuid()}";
        }
        
        /// <summary>
        /// Per Moralis, if you change this value, you have to auth again. ("Cookie" is lost)
        /// </summary>
        /// <returns></returns>
        public static string GetNewProductName()
        {
            // Running in a parelsync clone vs the original
            string clone = "";
            if (ClonesManagerWrapper.IsClone)
            {
                clone = "_Clone";
            }
            
            // Running inside the unity editor vs a build?
            string unityEditor = "";
            if (Application.isEditor)
            {
                unityEditor = "_UnityEditor";
            }

            return $"{ProjectNameShort}{unityEditor}{clone}";

        }
    }
}