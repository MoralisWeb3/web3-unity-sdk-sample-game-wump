
using RMC.Shared.Exceptions;
using MoralisUnity.Samples.TheGame.MVCS.Networking.MultiplayerSetupService;
using Unity.Netcode.Transports.UTP;

namespace MoralisUnity.Samples.TheGame.MVCS.Service.MultiplayerSetupService
{
    /// <summary>
    /// Create ONE from the set of mutually exclusive services depending
    /// on the related type passed in.
    ///
    /// See <see cref="MultiplayerSetupServiceType"/>
    /// </summary>
    public class MultiplayerSetupServiceFactory
    {
        //  Properties ------------------------------------

        //  Methods ---------------------------------------
        public IMultiplayerSetupService CreateMultiplayerSetupService(
            MultiplayerSetupServiceType multiplayerSetupServiceType,
            UnityTransport unityTransport, 
            bool isLanAutoStart,
            UnityTransport.SimulatorParameters lanSimulatorParameters)
        {
            IMultiplayerSetupService multiplayerSetupService = null;
            
            switch (multiplayerSetupServiceType)
            {
                case MultiplayerSetupServiceType.Lan:
                    
                    multiplayerSetupService = new LanMultiplayerSetupService(unityTransport, 
                        isLanAutoStart, lanSimulatorParameters);
                    
                    break;
                case MultiplayerSetupServiceType.Full:
                    
                    multiplayerSetupService = new FullMultiplayerSetupService(unityTransport);
                    
                    break;
                default:
                    SwitchDefaultException.Throw(multiplayerSetupServiceType);
                    break;
            }

            return multiplayerSetupService;
        }
    }
}