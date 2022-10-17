using System;
using System.Collections.Generic;
using System.Linq;
using MoralisUnity.Samples.Shared;
using MoralisUnity.Samples.Shared.UI;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;
using MoralisUnity.Samples.TheGame.MVCS.Model;
using MoralisUnity.Samples.TheGame.MVCS.View;
using MoralisUnity.Samples.TheGame.MVCS.View.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MoralisUnity.Samples.TheGame
{
    /// <summary>
    /// Helper Methods
    /// </summary>
    public static class TheGameHelper
    {
                
        // Fields -----------------------------------------

        public const string GiftGold = "GOLD";
        public const string GiftPrize = "PRIZE";

        
        // General Methods --------------------------------
        
        public static PlayerView GetPlayerViewByClientId(ulong clientId)
        {
            return NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerView>();
        }
        
        public static uint GetGiftType(string name)
        {
            if (name == GiftGold)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public static string GetGiftTypeNameByType(uint t)
        {
            if (t == 1)
            {
                return "Gold";
            }
            else
            {
                return "Prize";
            }
        }
        
        public static int GetAudioClipIndexClick()
        {
            return 0;
        }
        
        public static int GetAudioClipIndexChestHit01()
        {
            return 1;
        }
        
        public static int GetAudioClipIndexChestHit02()
        {
            return 2;
        }
        
        public static int GetAudioClipIndexWinSound()
        {
            return 3;
        }
        
        public static int GetAudioClipIndexByReward(TransferLog transferLog)
        {
            if (transferLog.Type == 1)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public static T InstantiatePrefab<T>(T prefab, Transform parent, Vector3 worldPosition) where T : Component
        {
            T instance = GameObject.Instantiate<T>(prefab, parent);

            instance.gameObject.name = instance.GetType().Name;
            instance.transform.position = worldPosition;
            return instance;
        }

        public static void SetButtonText(Button button, bool isActive, string activeText, string notActiveText)
        {
            if (isActive)
            {
                SetButtonText(button, activeText);
            }
            else
            {
                SetButtonText(button, notActiveText);
            }
        }
        
        public static void SetButtonText(Button button, string text)
        {
            TMP_Text tmp_Text = button.GetComponentInChildren<TMP_Text>();
            tmp_Text.text = text;
        }

        /// <summary>
        /// Add custom parsing outside of class hierarchy (thus, use static)
        /// </summary>
        public static string ConvertMetadataObjectToString (PrizeMetadata prizeMetadata)
        {
            return $"ImageUrl={prizeMetadata.ImageUrl}";
        }

        /// <summary>
        /// Add custom parsing outside of class hierarchy (thus, use static)
        /// </summary>
        public static PrizeMetadata ConvertMetadataStringToObject(string result)
        {
            //  Sometimes we store many values packed into the NFT uri
            //  Here we store just one
            //  So no split-parsing needed
            //
            // List<string> tokens = result.Split("|").ToList();
            // string title = tokens[0].Split("=")[1];
            // uint price = uint.Parse(tokens[1].Split("=")[1]);

            if (result.Length ==0)
            {
                throw new ArgumentException();
            }

            return new PrizeMetadata
            {
                ImageUrl = result
            };
        }
        public static TransferLog ConvertTransferLogStringToObject(string result)
        {
            List<string> tokens = result.Split("|").ToList();
            string fromAddress = tokens[0].Split("=")[1];
            string toAddress = tokens[1].Split("=")[1];
            uint type = uint.Parse(tokens[2].Split("=")[1]);
            uint amount = uint.Parse(tokens[3].Split("=")[1]);

            if (fromAddress.Length == 0 || toAddress.Length == 0 || type == 0 || amount == 0)
            {
                Debug.Log("error with : " + result);
                throw new ArgumentException();
            }
            
            return new TransferLog
            {
                FromAddress = fromAddress,
                ToAddress = toAddress,
                Type = type,
                Amount = amount,
            };
        }

        public static string FormatGoldCornerText(int amount)
        {
            return string.Format("{000:000}/{001:000}", amount, TheGameConstants.GoldMax);
        }

        public static string FormatPrizeCornerText(int amount)
        {
            return string.Format("{000:00}/{001:00}", amount, TheGameConstants.PrizesMax);
        }

        public static string GetTransferLogDisplayText(TransferLog transferLog)
        {
            string fromAddress = CustomWeb3System.Instance.ConvertWeb3AddressToShortFormat(transferLog.FromAddress);
            string toAddress = CustomWeb3System.Instance.ConvertWeb3AddressToShortFormat(transferLog.ToAddress);
            string type = GetGiftTypeNameByType(transferLog.Type);
            string amount = transferLog.Amount.ToString();
            return $"Transfer complete for {amount} {type}\n<size=30>From: {fromAddress}\nTo: {toAddress}</size>";
        }

        public static string GetRandomizedNickname()
        {
            List<string> article = new List<string>
            {
                "The",
                "A",
                "One"
            };
            
            List<string> adjective = new List<string>
            {
                "Nice",
                "Angry",
                "Mean",
                "Fun",
                "Little",
                "Big"
            };
            
            List<string> noun = new List<string>
            {
                "Friend",
                "Enemy",
                "Giant",
                "Shrimp",
                "Boss",
            };

            return $"{article[UnityEngine.Random.Range(0, article.Count)]} {adjective[UnityEngine.Random.Range(0, adjective.Count)]} {noun[UnityEngine.Random.Range(0, noun.Count)]}";
        }

        public static string GetPlayerNameAsSceneTitle(string title, int fontSize = 9)
        {
            return $"<size=9>\n</size><size={fontSize}>{title}</size>";
        }

        public static void SetShutdownButtonText(BaseButtonUI button, bool isConnected, bool isHost)
        {
            if (isConnected)
            {
                if (isHost)
                {
                    button.Text.text = $"<size=30>Leave</size><size=20> (Host)</size>";
                }
                else
                {
                    button.Text.text = $"<size=30>Leave</size><size=20> (Client)</size>";
                }
            }
            else
            {
                button.Text.text = $"<size=30>Leave</size><size=20></size>";
            }
        }
    }
}