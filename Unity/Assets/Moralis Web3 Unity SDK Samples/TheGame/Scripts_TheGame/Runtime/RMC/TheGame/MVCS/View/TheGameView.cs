using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RMC.TheGame.MVCS.View
{
        //  Namespace Properties ------------------------------


        //  Class Attributes ----------------------------------


        /// <summary>
        /// The core screen UI logic that is NOT networking
        /// </summary>
        public class TheGameView : MonoBehaviour
        {
            //  Events ----------------------------------------
            [HideInInspector]
            public UnityEvent OnSharedStatusUpdateRequested = new UnityEvent();

            //  Properties ------------------------------------
            public string PlayerName
            {
                get
                {
                    return _playerName;
                }
                set
                {
                    _playerName = value;
                    UpdatePlayerName();
                }
            }
            
            public string LocalStatus
            {
                get
                {
                    return _localStatus;
                }
                set
                {
                    _localStatus = value;
                    UpdateLocalStatusAsync();
                }
            }
            
            public string SharedStatus
            {
                get
                {
                    return _sharedStatus;
                }
                set
                {
                    _sharedStatus = value;
                    UpdateSharedStatusAsync();
                }
            }
            
            public static TheGameView Instance
            {
                get
                {
                    return _Instance;
                }
            }
            
            //  Fields ----------------------------------------
            [SerializeField] 
            private TMP_Text _playerDetailsText;

            [SerializeField] 
            private TMP_Text _statusText;
            
            private readonly StringBuilder _statusStringBuilder = new StringBuilder();
            private string _localStatus = "";
            private string _playerName = "";
            private string _sharedStatus = "";
            private const int FlickerDelayMilliseconds = 100;

            private static TheGameView _Instance;
            
            //  Unity Methods ---------------------------------
            protected void Awake()
            {
                _Instance = this;
            }

            //  Methods ---------------------------------------
            
            private void UpdatePlayerName()
            {
                _playerDetailsText.text = _playerName; 
            }

            private async void UpdateSharedStatusAsync()
            {
                // Flicker off - to make changes obvious
                UpdateStatusInternal("", _localStatus);
                await Task.Delay(FlickerDelayMilliseconds);
                
                // Flick on
                UpdateStatusInternal(_sharedStatus, _localStatus);
            }

            private async void UpdateLocalStatusAsync()
            {
                // Flicker off - to make changes obvious
                UpdateStatusInternal(_sharedStatus, "");
                await Task.Delay(FlickerDelayMilliseconds);
                
                // Flick on
                UpdateStatusInternal(_sharedStatus, _localStatus);
            }

            private void UpdateStatusInternal(string sharedStatus, string localStatus)
            {
                // Flicker on
                _statusStringBuilder.Clear();
                _statusStringBuilder.AppendLine("<b>Status</b>");
                _statusStringBuilder.AppendLine($" Local: {localStatus}");
                _statusStringBuilder.AppendLine($" Shared: {sharedStatus}");
                _statusText.text = _statusStringBuilder.ToString();
            }
        
            
            //  Event Handlers --------------------------------
            public void SharedStatusUpdateRequest()
            {
                OnSharedStatusUpdateRequested.Invoke();
            }
        }
    }
