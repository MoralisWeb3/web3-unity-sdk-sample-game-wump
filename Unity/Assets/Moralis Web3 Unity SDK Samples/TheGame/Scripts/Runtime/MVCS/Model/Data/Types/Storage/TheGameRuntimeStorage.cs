using MoralisUnity.Samples.Shared.Data.Types.Storage;
using MoralisUnity.Samples.Shared.Events;
using UnityEngine;

namespace MoralisUnity.Samples.Web3MagicTreasureChest.MVCS.Model.Data.Types.Storage
{
    /// <summary>
    /// Example local storage which is written to disk
    /// and used throughout various examples to hold shared state.
    /// </summary>
    public class TheGameRuntimeStorage : BaseRuntimeStorage<TheGameRuntimeStorage>
    {

        //  Events  ---------------------------------------
        [HideInInspector] public StringUnityEvent OnActiveAddressChanged = new StringUnityEvent();


        //  Properties  -----------------------------------
        public bool HasActiveAddress
        {
            get { return !string.IsNullOrEmpty(_activeAddress); }
        }

        public string ActiveAddress
        {
            get { return _activeAddress; }
            set
            {

                bool isChanging = _activeAddress != value;
                _activeAddress = value;
                OnActiveAddressChanged.Invoke(_activeAddress);
            }
        }

        public bool HasSceneNamePrevious
        {
            get { return !string.IsNullOrEmpty(SceneNamePrevious); }
        }

        public string SceneNamePrevious
        {
            get { return _sceneNamePrevious; }
            set
            {
                bool isChanging = _sceneNamePrevious != value;
                _sceneNamePrevious = value;
            }
        }


        //  Fields  ---------------------------------------
        [SerializeField] 
        private string _activeAddress = "";

        [SerializeField] 
        private string _sceneNamePrevious = "";


        //  Unity Methods  --------------------------------


        //  General Methods  ------------------------------
        public override void InstantiateCompleted()
        {
            //Debug.Log("InstantiateCompleted()");
        }

        public void ResetSceneNamePrevious()
        {
            SceneNamePrevious = string.Empty;
        }
    }
}


