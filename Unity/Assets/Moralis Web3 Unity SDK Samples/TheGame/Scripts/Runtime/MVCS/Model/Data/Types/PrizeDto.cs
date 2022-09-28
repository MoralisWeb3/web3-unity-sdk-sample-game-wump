
using System;
using MoralisUnity.Samples.Shared.Data.Types;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types
{
	/// <summary>
	/// Stores data for the game
	/// </summary>
	[Serializable]
	public class PrizeDto : Nft
	{
		// Properties -------------------------------------


		// Fields -----------------------------------------


		// Initialization Methods -------------------------
		public PrizeDto(string ownerAddress, string metadata) : base (ownerAddress, metadata)
		{
		}
		public PrizeDto() : base ()
		{
		}

		// General Methods --------------------------------


		// Event Handlers ---------------------------------

	}
}
