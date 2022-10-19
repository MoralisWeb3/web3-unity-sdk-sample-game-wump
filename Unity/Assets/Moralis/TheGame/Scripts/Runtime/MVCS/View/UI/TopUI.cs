using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.Shared.Helpers;
using TMPro;
using UnityEngine;
#pragma warning disable CS0618
namespace MoralisUnity.Samples.TheGame.MVCS.View.UI
{
	public class Message
	{
		public string Text = "";
		public int DurationMilliseconds = 1000;

		public Message(string text, int durationMilliseconds)
		{
			Text = text;
			DurationMilliseconds = durationMilliseconds;
		}
	}
	/// <summary>
	/// Handles the top navigation
	/// </summary>
	public class TopUI : MonoBehaviour
	{
		// Properties -------------------------------------
		public CornerUI GoldCornerUI { get { return _goldCornerUI; }}
		public CornerUI PrizeCornerUI { get { return _prizeCornerUI; } }
		public TMP_Text SharedStatusText { get { return _sharedStatusText; } }
		
		// Fields -----------------------------------------
		[Header("References (Scene)")] 
		[SerializeField] 
		private CornerUI _goldCornerUI = null;

		[SerializeField]
		private CornerUI _prizeCornerUI = null;

		[SerializeField]
		private TMP_Text _sharedStatusText = null;

		[SerializeField]
		private ImageAndCanvasView _imageAndCanvasView = null;

		private CancellationTokenSource _cancellationTokenSource;
		private Queue<Message> _messageQueue = new Queue<Message>();

		//  Unity Methods----------------------------------
		protected void Start()
		{
			ProcessStatusMessageQueue();
		}
		
		// General Methods --------------------------------
		public void QueueSharedStatusText(string text, int durationMilliseconds)
		{
			_messageQueue.Enqueue(new Message(text, durationMilliseconds));
		}
		
		private async void ProcessStatusMessageQueue()
		{
			await UniTask.RunOnThreadPool(async () =>
			{
				while (true)
				{
					while (_messageQueue.Count > 0)
					{
						
						await EnsureFadeAsync(0, 1);
						Message message = _messageQueue.Dequeue();
						await SetTextAsync(message.Text);
						await UniTask.Delay(message.DurationMilliseconds);
					}

					if (HasText())
					{
						await SetTextAsync("");
						await EnsureFadeAsync(1, 0);
					}

					await UniTask.WaitForEndOfFrame();

				}
			});

		}

		private async UniTask EnsureFadeAsync(float fromAlpha, float toAlpha)
		{
			await UniTask.SwitchToMainThread();
			if (_imageAndCanvasView.Alpha != toAlpha)
			{
				await TweenHelper.AlphaDoFade(_imageAndCanvasView, fromAlpha, toAlpha, 0.1f, 0, Ease.InOutSine);
			}
		}

		private bool HasText()
		{
			return !string.IsNullOrEmpty(_sharedStatusText.text);
		}
		private async UniTask SetTextAsync(string text)
		{
			await UniTask.SwitchToMainThread();
			_sharedStatusText.text = text;
		}

		// Event Handlers ---------------------------------


	}
}
