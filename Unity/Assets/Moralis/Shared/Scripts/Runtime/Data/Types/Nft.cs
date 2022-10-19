using System;
using Newtonsoft.Json;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.Data.Types
{
	/// <summary>
	/// Represents all data for one nft 
	/// </summary>
	[Serializable]
	public class Nft 
	{
		// Properties -------------------------------------
		public string OwnerAddress { get { return _ownerAddress;}}
		public int TokenId { get { return _tokenId;}}
		public string Metadata { get { return _metadata; } }

		// Fields -----------------------------------------
		[SerializeField]
		private string _ownerAddress;
		
		[SerializeField]
		private int _tokenId;

		[SerializeField]
		private string _metadata = "";

		public const int NullTokenId = -1;

		// Initialization Methods -------------------------

		/// <summary>
		/// Created from view by user gesture
		/// </summary>
		public Nft(string ownerAddress, string metadata)
		{
			Initialize(ownerAddress, NullTokenId, metadata);
		}
		
		public Nft()
		{
		
		}
		
		/// <summary>
		/// Created from service by loading data
		/// </summary>
		[JsonConstructor]
		public Nft(string ownerAddress, int tokenId, string metadata)
		{
			Initialize(ownerAddress, tokenId,  metadata);
		}
		
		private void Initialize (string ownerAddress, int tokenId, string metadata)
		{
			_ownerAddress = ownerAddress;
			_tokenId = tokenId;
			_metadata = metadata;
		}
		
		// General Methods --------------------------------

		public static T CreateNewFromMetadata<T>(string ownerAddress, string newTokenIdString, string metadata) where T : Nft, new()
		{
			int tokenId = NullTokenId;
			
			//TODO: remove this validator. That is checking for address but we are using an 'id'. right?
			if (!string.IsNullOrEmpty(newTokenIdString) )
			{
				tokenId = int.Parse(newTokenIdString);
			}

			T t = new T();
			t.Initialize(ownerAddress, tokenId,  metadata);
			return t;
		}

		public override string ToString()
		{
			return $"[{GetType().Name}(_ownerAddress = {_ownerAddress}, _tokenId = {_tokenId}, _metadata = {_metadata})]";
		}

		// Event Handlers ---------------------------------

	}
}
