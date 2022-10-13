
namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types
{
    /// <summary>
    /// Observable<t> does not like 'string'. So I created a wrapper class.
    /// </summary>
    public class CustomPlayerInfo
    {
	    public string Nickname = "";
	    public string Web3Address = "";

	    public bool HasNickname
	    {
		    get { return !string.IsNullOrEmpty(Nickname); }
	    }
	    
	    public bool HasWeb3Address
	    {
		    get { return !string.IsNullOrEmpty(Web3Address); }
	    }

	    public CustomPlayerInfo()
        {
		
        }

        
    }
}