using System.Collections.Generic;
using MoralisUnity.Samples.Shared.Data.Types.Configuration;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.Audio
{
	/// <summary>
	/// Maintain a list of AudioSources and play the next 
	/// AudioClip on the first available AudioSource.
	/// </summary>
	[CreateAssetMenu( menuName = SharedConstants.PathMoralisSharedCreateAssetMenu + Title,  fileName = Title, order = SharedConstants.PriorityMoralisTools_Primary)]
	public class SoundManagerConfiguration: BaseConfiguration<SoundManagerConfiguration>
	{
	 	// Properties -------------------------------------
		public List<AudioClip> AudioClips { get { return _audioClips; } }
		
		
		//  Fields ----------------------------------------
		private const string Title = "SoundManagerConfiguration";
	
		[SerializeField]
		private List<AudioClip> _audioClips = new List<AudioClip>();

	}
}