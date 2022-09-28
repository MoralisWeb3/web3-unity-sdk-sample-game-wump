using System;
using Cysharp.Threading.Tasks;
using MoralisUnity.Samples.Shared.Audio;
using MoralisUnity.Samples.Shared.Components;
using MoralisUnity.Samples.Shared.Helpers;
using MoralisUnity.Samples.Shared.UI;
using UnityEngine;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.View
{
	/// <summary>
	/// Handles the UI for the game
	///		* See <see cref="TheGameSingleton"/>
	/// </summary>
	public class TheGameView : MonoBehaviour
	{
		// Properties -------------------------------------
		public SceneManagerComponent SceneManagerComponent { get { return _sceneManagerComponent;}}
		public BaseScreenMessageUI BaseScreenCoverUI { get { return _baseScreenMessageUI; }}
		
		
		// Fields -----------------------------------------
		[Header("References (Scene)")] 
		[SerializeField] 
		private SceneManagerComponent _sceneManagerComponent = null;
		
		[SerializeField] 
		private BaseScreenMessageUI _baseScreenMessageUI = null;

		private static readonly Vector3 SmallScale = new Vector3(.75f, .75f, .75f);
		private static readonly Vector3 FullScale = new Vector3(1, 1, 1);
		
		//[Header("References (Project)")] 
	
		// General Methods --------------------------------
		/// <summary>
		/// Show a loading screen, during method execution
		/// </summary>
		public async UniTask ShowMessageDuringMethodAsync(
			string message, 
			Func<UniTask> task)
		{
			// Empty Message As it fades in
			BaseScreenCoverUI.MessageText.text = "";
			
			BaseScreenCoverUI.BlocksRaycasts = true;
			
			TweenHelper.TransformDoScale(BaseScreenCoverUI.Panel, 
				SmallScale, FullScale, 0.25f, 0);
			await TweenHelper.AlphaDoFade(BaseScreenCoverUI, 0, 1, 0.25f);
			
			UpdateMessageDuringMethod(message, false);
			await task();
			
			TweenHelper.TransformDoScale(BaseScreenCoverUI.Panel, 
				FullScale, SmallScale, 0.25f, 0);
			await TweenHelper.AlphaDoFade(BaseScreenCoverUI, 1, 0, 0.25f);
			
			BaseScreenCoverUI.BlocksRaycasts = false;
			
	
		}
		
		public async UniTask ShowMessageWithDelayAsync(string message, int delayMilliseconds)
		{
			// Empty Message As it fades in
			BaseScreenCoverUI.MessageText.text = "";
			
			BaseScreenCoverUI.BlocksRaycasts = true;
			
			TweenHelper.TransformDoScale(BaseScreenCoverUI.Panel, 
				SmallScale, FullScale, 0.25f, 0);
			await TweenHelper.AlphaDoFade(BaseScreenCoverUI, 0, 1, 0.25f);
			
			UpdateMessageDuringMethod(message, false);
			await UniTask.Delay(delayMilliseconds);
			
			TweenHelper.TransformDoScale(BaseScreenCoverUI.Panel, 
				FullScale, SmallScale, 0.25f, 0);
			await TweenHelper.AlphaDoFade(BaseScreenCoverUI, 1, 0, 0.25f);
			
			BaseScreenCoverUI.BlocksRaycasts = false;
			
		
		}
		
		public void UpdateMessageDuringMethod(string message, bool isAnimated = true)
		{
			if (isAnimated)
			{
				TweenHelper.TransformDoScale(BaseScreenCoverUI.Panel,
					FullScale, SmallScale, 0.25f, 0).onComplete = () =>
				{
					BaseScreenCoverUI.MessageText.text = message;
			
					TweenHelper.TransformDoScale(BaseScreenCoverUI.Panel, 
						SmallScale, FullScale, 0.25f, 0);
				};
			}
			else
			{
				BaseScreenCoverUI.MessageText.text = message;
			}
		}


		/// <summary>
		/// Play generic click sound
		/// </summary>
		public void PlayAudioClipClick()
		{
			SoundManager.Instance.PlayAudioClip(TheGameHelper.GetAudioClipIndexClick());
		}
		
		public void PlayAudioClip(int audioClipIndex)
		{
			SoundManager.Instance.PlayAudioClip(audioClipIndex);
		}
		
		// Event Handlers ---------------------------------



	}
}
