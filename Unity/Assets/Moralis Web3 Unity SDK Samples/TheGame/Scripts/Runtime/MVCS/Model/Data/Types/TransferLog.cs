namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types
{
    /// <summary>
    /// The reward the server gives to the player
    /// </summary>
    public class TransferLog 
    {
        public string FromAddress;
        public string ToAddress;
        public uint Type; 
        public uint Amount;

        public override string ToString()
        {
            return $"[{GetType().Name}(FromAddress = {FromAddress}, ToAddress = {ToAddress}, Type = {Type}, Amount = {Amount})]";
        }
    }
}