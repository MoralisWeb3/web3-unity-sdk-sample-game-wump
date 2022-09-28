
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types
{
    [Serializable]
    public class TreasurePrizeMetadata
    {
        public string Title = "";
        public uint Price = 0;
    }

    /// <summary>
    /// Stores data for the game
    /// </summary>
    [Serializable]
    public class TreasurePrizeDto : PrizeDto
    {
        // Properties -------------------------------------
        public string Title { get { return TheGameHelper.ConvertMetadataStringToObject(Metadata).Title; } }
        public uint Price { get { return TheGameHelper.ConvertMetadataStringToObject(Metadata).Price; } }

        // Fields -----------------------------------------


        // Initialization Methods -------------------------
        public TreasurePrizeDto (string ownerAddress, string metadata) : base(ownerAddress, metadata) 
        {

        }
        public TreasurePrizeDto () : base() 
        {

        }
        // General Methods --------------------------------



        // Event Handlers ---------------------------------

    }
}
