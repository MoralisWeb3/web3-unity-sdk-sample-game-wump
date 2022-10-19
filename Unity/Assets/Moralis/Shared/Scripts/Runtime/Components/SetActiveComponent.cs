using UnityEngine;

namespace MoralisUnity.Samples.Shared.Components
{
    /// <summary>
    /// Replace with comments...
    /// </summary>
    public class SetActiveComponent : MonoBehaviour
    {
        //  Properties ------------------------------------
        public bool IsActiveOnAwake { get { return _isActiveOnAwake; } }

		
        //  Fields ----------------------------------------
        [SerializeField]
        private bool _isActiveOnAwake = true;

        
        //  Unity Methods----------------------------------
        protected void Awake()
        {
            gameObject.SetActive(_isActiveOnAwake);
        }


        //  General Methods -------------------------------

		
        //  Event Handlers --------------------------------
    }
}