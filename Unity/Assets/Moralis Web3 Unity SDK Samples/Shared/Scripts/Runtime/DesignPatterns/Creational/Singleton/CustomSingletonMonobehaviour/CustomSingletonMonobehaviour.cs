using System;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.DesignPatterns.Creational.Singleton.CustomSingletonMonobehaviour
{
    public abstract class CustomSingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        //  Properties ------------------------------------------
        public static bool IsShuttingDown
        {
            get
            {
                return _IsShuttingDown;
            }
        }
        
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
        private static bool _IsShuttingDown = false;
        public delegate void OnInstantiateCompletedDelegate(T instance);
        public static OnInstantiateCompletedDelegate OnInstantiateCompleted;

        //  Instantiation ------------------------------------------

        public static T Instantiate()
        {
            if (IsShuttingDown)
            {
                throw new Exception("Must check IsShuttingDown before calling Instantiate/Instance.");
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
        protected void OnDestroy()
        {
            Destroy();
        }
        
        /// <summary>
        /// Detect and solve corner case
        /// </summary>
        public static void Destroy()
        {
            _IsShuttingDown = true;
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