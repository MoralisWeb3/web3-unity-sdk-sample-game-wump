using System;
using System.Collections;
using System.Collections.Generic;
using MoralisUnity.Samples.Shared.Attributes;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0414
namespace MoralisUnity.Samples.Shared.Data.Types
{
    public class ObservableUnityEvent<T> : UnityEvent<T> where T : new()
    {
    }

    /// <summary>
    /// Wrapper so a type can be observable via events
    /// </summary>
    public class Observable<T> where T : new()
    {
        //  Events ----------------------------------------
        public ObservableUnityEvent<T> OnValueChanged = new ObservableUnityEvent<T>();
        
        //  Properties ------------------------------------
        public T Value
        {
            set
            {
                _value = OnValueChanging(_value, value);
                OnValueChanged.Invoke(_value);
            }
            get
            {
                return _value;
                
            }
        }

        //  Fields ----------------------------------------
        [InspectorComment("Note: Value shown for debugging. Do not edit via inspector.")]
        [SerializeField]
        private string _dummyInspectorComment = "";
        
        [SerializeField]
        private T _value;
        
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
        
        //  Event Handlers --------------------------------
    }
}