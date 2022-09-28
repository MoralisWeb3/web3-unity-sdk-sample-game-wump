namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types
{
    /// <summary>
    /// The reward the server gives to the player
    /// </summary>
    public class Reward 
    {
        public string Title;
        public uint Type;
        public uint Price;

        public override string ToString()
        {
            return $"[{GetType().Name}(Title = {Title}, Type = {Type}, Price = {Price})]";
        }
    }
}