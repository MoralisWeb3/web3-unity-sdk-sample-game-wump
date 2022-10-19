using MoralisUnity.Samples.Shared.Debugging;
using MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingletonScriptableObject;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.Data.Types.Configuration
{
    /// <summary>
    /// Main configuration for the game. Click the instance of this class in the project to view/edit
    /// </summary>
    public class BaseConfiguration <T>  : CustomSingletonScriptableObject<T> where T : ScriptableObject
    {
        // Properties -------------------------------------
        public bool IsLogEnabled { get { return _isLogEnabled;}}
  
        // Fields -----------------------------------------
        [Header("Settings")]
        [SerializeField]
        private bool _isLogEnabled = true;

        // Unity Methods ----------------------------------
        protected virtual void OnValidate()
        {
            if (Custom.Debug.IsEnabled != _isLogEnabled)
            {
                Custom.Debug.IsEnabled = _isLogEnabled;
            }
        }
        // General Methods --------------------------------

        // Event Handlers ---------------------------------
    }
}