using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Samples.Shared.Debugging;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MoralisUnity.Samples.Shared
{
    /// <summary>
    /// This helper hides complex-but-required code
    /// which is outside the educational scope of per-scene
    /// example files
    /// </summary>
    public static class SharedHelper
    {
        //  General Methods  --------------------------------------
        
        /// <summary>
        /// Convert from visuals into string
        /// </summary>
        public static string ConvertSpriteToContentString(Sprite sprite)
        {
            Texture2D mytexture = sprite.texture;
            byte[] bytes;
            bytes = mytexture.EncodeToPNG();
            return Convert.ToBase64String(bytes);
        }
        
        
        /// <summary>
        /// Convert from URl of string into visuals
        /// </summary>
        public static async UniTask<Sprite> CreateSpriteFromImageUrl(string path)
        {   
            var getRequest = UnityWebRequest.Get(path);
            await getRequest.SendWebRequest();

            Texture2D textured2D = new Texture2D(2, 2);
            byte[] bytes = getRequest.downloadHandler.data;

            // Empty bytes means empty image. Support that.
            if (bytes != null && bytes.Length > 0)
            { 
                textured2D.LoadImage(bytes);
            }
            
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(textured2D, new Rect(0.0f, 0.0f, textured2D.width, textured2D.height), pivot, 100.0f);
            return sprite;
        }

        /// <summary>
        /// DontDestroyOnLoad works only if parent==null
        /// </summary>
        /// <param name="gameObject"></param>
        public static void SafeDontDestroyOnLoad(GameObject go)
        {
            go.transform.SetParent(null);
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        public static Image CreateNewImageUnderParentAsLastSibling(Transform parent, Vector2 preferredSize)
        {
            Vector2 spacerSize = new Vector2(100, 100);
            
            // Put space above
            GameObject spaceGo1 = 
                CreateLayoutUnderParentAsLastSibling("spacer", parent, spacerSize);

            // Make image
            GameObject imageGo = 
                CreateLayoutUnderParentAsLastSibling("newImage", parent, preferredSize);
            Image image = imageGo.AddComponent<Image>();
            image.preserveAspect = true;
            
            // Put space below
            GameObject spaceGo2 = 
                CreateLayoutUnderParentAsLastSibling("spacer", parent, spacerSize);

            return image;
        }
        
        public static GameObject CreateLayoutUnderParentAsLastSibling(string newName, Transform parent, Vector2 preferredSize)
        {
            GameObject go = new GameObject(newName);
            LayoutElement layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = preferredSize.x;
            layoutElement.preferredHeight = preferredSize.y;
            go.transform.SetParent(parent);
            go.transform.SetSiblingIndex(go.transform.childCount-1);
            CanvasGroup canvasGroup = go.AddComponent<CanvasGroup>();
            return go;
        }

        
        public static List<string> ConvertTextAssetToLines(TextAsset textAsset, int startLineIndex = 0)
        {
            var separator = new string[] { "\r\n", "\r", "\n" };
            var sourceLines = textAsset.text.Split(separator, StringSplitOptions.None).ToList();
            List<string> destinationLines = new List<string>(); 
            for (int l = startLineIndex; l < sourceLines.Count; l++)
            {
                destinationLines.Add(sourceLines[l]);
            }

            return destinationLines;
        }

        
        /// <summary>
        /// Wait X seconds so user sees the noticeable feedback
        /// flicker of "Loading..." in the text box
        /// </summary>
        public static async UniTask TaskDelayWaitForCosmeticEffect()
        {
            await UniTask.Delay(100);
        }

        /// <summary>
        /// Wait 1 frame for UI to render
        /// </summary>
        public static async UniTask TaskDelayWaitForEndOfFrame2()
        {
            await UniTask.WaitForEndOfFrame();
        }
        
        /// <summary>
        /// Changes "blah_blah" to "Blah Blah"
        /// </summary>
        /// <param name="chainEntry"></param>
        /// <returns></returns>
        public static string GetPrettifiedNameByChainEntry(ChainEntry chainEntry)
        {
	        string name = chainEntry.Name.Replace("_", " ");
	        name = ToTitleCase(name);
	        return name;
        }

        
        private static string ToTitleCase(string message)
        {
	        TextInfo myTI = new CultureInfo("en-US",false).TextInfo;
	        return myTI.ToTitleCase(message);
        }

        
        /// <summary>
        /// Call Moralis Servers and get the current server time.
        /// This is used for specific use-cases including EthSign.
        /// </summary>
        /// <returns></returns>
        public static async UniTask<long> GetServerTime()
        {
            long serverTime = 0;
            // Get Server Time (Needed for EthSign)
            Dictionary<string, object> serverTimeResponse = 
                await Moralis.GetClient().Cloud.RunAsync<Dictionary<string, object>>("getServerTime", new Dictionary<string, object>());
            if (serverTimeResponse == null || 
                !serverTimeResponse.ContainsKey("dateTime") ||
                !long.TryParse(serverTimeResponse["dateTime"].ToString(), out serverTime))
            {
                Custom.Debug.Log("Failed to retrieve server time from Moralis Server!");
            }

            return serverTime;
        }

        
        /// <summary>
        /// Determines if Moralis is logged in with an active user.
        /// </summary>
        /// <returns></returns>
        public static async UniTask<bool> HasMoralisUser()
        {
            MoralisUser moralisUser = await Moralis.GetUserAsync();
            return moralisUser != null;
        }

    }
}