using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

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
			await UniTask.Run(async () =>
			{
				while (true)
				{
					while (_messageQueue.Count > 0)
					{
						Debug.Log(_messageQueue.Count);
						Message message = _messageQueue.Dequeue();
						SetText(message.Text);
						await UniTask.Delay(message.DurationMilliseconds);
					}

					if (HasText())
					{
						SetText("");
					}
					await UniTask.WaitForEndOfFrame();
				}
			});

		}

		private bool HasText()
		{
			return !string.IsNullOrEmpty(_sharedStatusText.text);
		}
		private void SetText(string text)
		{
			_sharedStatusText.text = text;
			Debug.Log(_sharedStatusText.text );
		}

		// Event Handlers ---------------------------------


	}
}
