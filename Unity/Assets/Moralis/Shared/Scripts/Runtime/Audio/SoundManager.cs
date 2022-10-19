using System;
using System.Collections.Generic;
using MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingletonMonobehaviour;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.Audio
{
	/// <summary>
	/// Maintain a list of AudioSources and play the next 
	/// AudioClip on the first available AudioSource.
	/// </summary>
	public class SoundManager : CustomSingletonMonobehaviour<SoundManager>
	{
		// Properties -------------------------------------
		public List<AudioClip> AudioClips { get { return _soundManagerConfiguration.AudioClips; } }
		
		// Fields -----------------------------------------
		[Header("References (Project)")]
		[SerializeField]
		private SoundManagerConfiguration _soundManagerConfiguration = null;

		// Unity Methods ----------------------------------
		[SerializeField]
		private List<AudioSource> _audioSources = new List<AudioSource>();
		
		// General Methods --------------------------------
		/// <summary>
		/// Play the AudioClip by index.
		/// </summary>
		public void PlayAudioClip(int audioClipIndex)
		{
			AudioClip audioClip = null;
			try
			{
				audioClip = AudioClips[audioClipIndex];
			}
			catch
			{
				throw new ArgumentException($"PlayAudioClip() failed for index = {audioClipIndex}");
			}
			
			PlayAudioClip(audioClip);
		}

		
		/// <summary>
		/// Play the AudioClip by reference.
		/// If all sources are occupied, nothing will play.
		/// </summary>
		private void PlayAudioClip(AudioClip audioClip)
		{
			foreach (AudioSource audioSource in _audioSources)
			{
				if (!audioSource.isPlaying)
				{
					audioSource.clip = audioClip;
					audioSource.Play();
					return;
				}
			}
		}
		
		
		// Event Handlers ---------------------------------

	}
}