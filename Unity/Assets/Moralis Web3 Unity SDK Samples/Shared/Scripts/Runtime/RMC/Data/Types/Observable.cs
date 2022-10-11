using UnityEngine;
using UnityEngine.Events;

namespace RMC.Shared.Data.Types
{
    public class ObservableUnityEvent<F,T> : UnityEvent<F,T> where F: new() where T : new()
    {
    }

    /// <summary>
    /// Wrapper so a type can be observable via events
    /// </summary>
    public class Observable<T> where T : new()
    {
        //  Events ----------------------------------------
        public ObservableUnityEvent<T,T> OnValueChanged = new ObservableUnityEvent<T,T>();
        
        //  Properties ------------------------------------
        public T Value
        {
            set
            {
                _oldValue = _value;
                
                if (!_oldValue.Equals(value))
                {
                    _value = OnValueChanging(_oldValue, value);
                    SetDirty();
                }
                
            }
            get
            {
                return _value;
                
            }
        }

        //  Fields ----------------------------------------
        
        /// <summary>
        /// Allow for public calls to force invoke in corner cases
        /// </summary>
        public void SetDirty()
        {
            OnValueChanged.Invoke(_oldValue, _value);
        }
        
        [SerializeField]
        private T _value;

        private T _oldValue; //limit reallocations by declaring here
        
        //  Constructor Methods ---------------------------
        public Observable ()
        {
            _value = default(T);
        }

        //  Methods ---------------------------------------
        protected virtual T OnValueChanging (T oldValue, T newValue)
        {
            return newValue;
        }

        public override string ToString()
        {
            return $"[{typeof(T).Name}(value = {_value.ToString()})]";
        }

        //  Event Handlers --------------------------------
 
    }
}