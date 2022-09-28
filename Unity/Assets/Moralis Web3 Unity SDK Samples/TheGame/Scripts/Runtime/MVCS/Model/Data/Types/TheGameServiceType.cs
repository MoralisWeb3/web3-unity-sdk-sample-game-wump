
namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types
{
	/// <summary>
	/// At edit-time, toggle this with the <see cref="TheGameConfiguration"/> in the Unity inspector.
	/// This determines if the game runs off of the Moralis database (development) or Web3 Contracts (production)
	/// </summary>
	public enum TheGameServiceType
	{
		Null,
		
		// Blockchain Smart Contract
		Contract,		
		
		// Custom solution to write local Json
		LocalDiskStorage	
	}
}
