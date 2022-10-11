
using System;
using MoralisUnity.Samples.Shared.Data.Types;

namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types
{
	[Serializable]
	public class PrizeMetadata
	{
		//Sometimes we store many values packed into the NFT uri
		//Here we store just one
		public string ImageUrl = "";
	}
	
	/// <summary>
	/// Stores data for the game
	/// </summary>
	[Serializable]
	public class Prize : Nft
	{
		// Properties -------------------------------------
		public string ImageUrl { get { return TheGameHelper.ConvertMetadataStringToObject(Metadata).ImageUrl; } }

		// Fields -----------------------------------------


		// Initialization Methods -------------------------
		public Prize (string ownerAddress, string metadata) : base(ownerAddress, metadata) 
		{

		}
		public Prize () : base() 
		{

		}

		// General Methods --------------------------------


		// Event Handlers ---------------------------------

	}
}
