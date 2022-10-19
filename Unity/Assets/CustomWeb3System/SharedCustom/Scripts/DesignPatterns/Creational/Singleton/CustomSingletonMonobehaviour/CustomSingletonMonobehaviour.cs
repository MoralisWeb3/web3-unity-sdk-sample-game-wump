using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingletonMonobehaviour
{

    public abstract class CustomSingletonMonobehaviour : MonoBehaviour
    {
        private static bool _IsShuttingDown = false;
        
        public static bool IsShuttingDown
        {
            get
            {
                return _IsShuttingDown;
            }
            internal set
            {
                _IsShuttingDown = value;
               // Debug.Log("777777 _IsShuttingDown: " + _IsShuttingDown);
            }
        }
        
#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        static void InitializeOnLoadMethod()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// This InitializeOnLoadMethod scheme is designed to
        /// Properly reset the IsShuttingDown each time play mode ends
        ///
        /// The IsShuttingDown helps prevent non-singletons in their OnDestroy()
        /// from accidentally calling singleton.Instantiate which causes issues
        /// 
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                //Debug.Log(1);
                IsShuttingDown = false;
            }
            else if (state == PlayModeStateChange.EnteredPlayMode)
            {
               // Debug.Log(2);
            }
            //Debug.Log(state);
        }
#endif
    }

    public abstract class CustomSingletonMonobehaviour<T> : CustomSingletonMonobehaviour  where T : MonoBehaviour
    {
        
        //  Properties ------------------------------------------
        
        public static bool IsInstantiated
        {
            get
            {
                return _Instance != null;
            }
        }

        public static T Instance
        {
            get
            {
                if (!IsInstantiated)
                {
                    Instantiate();
                }
                return _Instance;
            }
            set
            {
                _Instance = value;
            }

        }

        //  Fields -------------------------------------------------
        private static T _Instance;
       
        public delegate void OnInstantiateCompletedDelegate(T instance);
        public static OnInstantiateCompletedDelegate OnInstantiateCompleted;

        //  Instantiation ------------------------------------------

        public static T Instantiate()
        {
            if (IsShuttingDown || !Application.isPlaying)
            {
                Debug.LogError("Must check IsShuttingDown before calling Instantiate/Instance.");
            }
            if (!IsInstantiated)
            {
                _Instance = GameObject.FindObjectOfType<T>();

                if (_Instance == null)
                {
                    GameObject go = new GameObject();
                    _Instance = go.AddComponent<T>();
                    go.name = _Instance.GetType().FullName;
                    DontDestroyOnLoad(go);
                }

                // Notify child scope
                (_Instance as CustomSingletonMonobehaviour<T>).InstantiateCompleted();

                // Notify observing scope(s)
                if (OnInstantiateCompleted != null)
                {
                    OnInstantiateCompleted(_Instance);
                }
            }
            return _Instance;
        }


        //  Unity Methods ------------------------------------------
        
        /// <summary>
        /// Detect and solve corner case
        /// </summary>
        protected void OnApplicationQuit()
        {
            Destroy();
        }
        
        /// <summary>
        /// Detect and solve corner case
        /// </summary>
        protected virtual void OnDestroy()
        {
            Destroy();
        }
        
        /// <summary>
        /// Detect and solve corner case
        /// </summary>
        public static void Destroy()
        {
            
            IsShuttingDown = true;
            //Debug.Log("77777777 Destroy: ");
            if (IsInstantiated)
            {
                if (Application.isPlaying)
                {
                    Destroy(_Instance.gameObject);
                }
                else
                {
                    DestroyImmediate(_Instance.gameObject);
                }

                _Instance = null;
            }
        }

        //  Other Methods ------------------------------------------
        public virtual void InstantiateCompleted()
        {
            // Override, if desired
        }

    }
}