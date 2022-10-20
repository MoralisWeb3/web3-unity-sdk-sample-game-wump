using MoralisUnity.Samples.Shared.Exceptions;
using MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types;

namespace MoralisUnity.Samples.TheGame.MVCS.Service.TheGameService
{
	/// <summary>
	/// Creates a concrete <see cref="ITheGameService"/>
	/// based on <see cref="TheGameServiceType"/>
	/// </summary>
	public class TheGameServiceFactory
	{
		// Properties -------------------------------------
		
		
		// Fields -----------------------------------------
		
		
		// General Methods --------------------------------
		public ITheGameService Create (TheGameServiceType theGameServiceType)
		{
			//KEEP LOG
			TheGameSingleton.Debug.LogBlueMessage($"TheGameServiceFactory() Using The Service For {theGameServiceType}");

			ITheGameService theGameService = null;
			switch (theGameServiceType)
			{
				case TheGameServiceType.Contract:
					theGameService = new TheGameContractService();
					break;
				case TheGameServiceType.LocalDiskStorage:
					theGameService = new LocalDiskStorageService();
					break;
				default:
					SwitchDefaultException.Throw(theGameServiceType);
					break;
			}

			return theGameService;
		}

		
		// Event Handlers ---------------------------------
	}
}
