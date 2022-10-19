#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	using System;
	using System.Collections.Generic;
	using Extension;
	using Issues;
	using Tools;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using UnityEngine.Events;
	using Object = UnityEngine.Object;

	internal enum UnityEventScanPhase
	{
		Begin = 0, // before UnityEvent processing
		Calls = 5, // m_PersistentCalls.m_Calls
		CallTarget = 10, // m_Calls[i].m_Target
		CallMethodName = 15, // m_Calls[i].m_MethodName
		ArgumentType = 20, // m_Calls[i].m_Arguments.m_ObjectArgumentAssemblyTypeName
		CallMode = 25, // m_Calls[i].m_Mode
		InvalidListener = 30 // UnityEventDrawer.IsPersistantListenerValid() == false
	}

	internal abstract class UnityEventScanner<T> : MaintainerExtension, IPropertyScanner<IUnityEventScanListener<T>, T>
		where T : IScanListenerResults
	{
		protected override bool Enabled
		{
			get { return true; }
			set { /* can't be disabled */ }
		}

		public IList<IUnityEventScanListener<T>> ScanListeners { get; protected set; }

		public Type ScanListenerType
		{
			get
			{
				return typeof(IUnityEventScanListener<T>);
			}
		}

		public void RegisterScanListeners(IUnityEventScanListener<T>[] listeners)
		{
			ScanListeners = listeners;
		}

		public void Property(T results, PropertyLocation location)
		{
			SendScanPhase(results, location, UnityEventScanPhase.Begin);
			//var innerLocation = location.Clone<PropertyLocation>();
			ProcessUnityEventCallbacks(results, location);
		}

		private void ProcessUnityEventCallbacks(T results, PropertyLocation location)
		{
			var calls = location.Property.FindPropertyRelative("m_PersistentCalls.m_Calls");
			if (calls == null || calls.isArray == false)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Couldn't find m_PersistentCalls in serialized UnityEvent!"));
				return;
			}
			
			SendScanPhase(results, location, UnityEventScanPhase.Calls, calls);

			var callsCount = calls.arraySize;
			for (var i = 0; i < callsCount; i++)
			{
				var call = calls.GetArrayElementAtIndex(i);
				var success = ProcessCall(results, location, call);
				if (!success)
					break;
			}
		}

		private bool ProcessCall(T results, PropertyLocation location, SerializedProperty call)
		{
			var callTarget = call.FindPropertyRelative("m_Target");
			if (callTarget == null || callTarget.propertyType != SerializedPropertyType.ObjectReference)
			{
				Debug.LogError(Maintainer.ErrorForSupport(
					"Couldn't find m_Target in serialized UnityEvent's call!"));
				return false;
			}
			
			SendScanPhase(results, location, UnityEventScanPhase.CallTarget, callTarget);

			var callTargetObject = callTarget.objectReferenceValue;

			// no target set (or target reference is missing)
			if (callTargetObject == null) 
				return true;

			var methodName = call.FindPropertyRelative("m_MethodName");
			if (methodName == null || methodName.propertyType != SerializedPropertyType.String)
			{
				Debug.LogError(Maintainer.ErrorForSupport(
					"Couldn't find m_MethodName in serialized UnityEvent's call!"));
				return false;
			}
			
			SendScanPhase(results, location, UnityEventScanPhase.CallMethodName, methodName);

			var methodNameValue = methodName.stringValue;

			// no function set
			if (string.IsNullOrEmpty(methodNameValue))
				return true;

			var arguments = call.FindPropertyRelative("m_Arguments");
			if (arguments == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport(
					"Couldn't find m_Arguments in serialized UnityEvent's call!"));
				return false;
			}

			var objectArgumentAssemblyTypeName = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
			if (objectArgumentAssemblyTypeName == null ||
				objectArgumentAssemblyTypeName.propertyType != SerializedPropertyType.String)
			{
				Debug.LogError(Maintainer.ErrorForSupport(
					"Couldn't find m_ObjectArgumentAssemblyTypeName in m_Arguments!"));
				return false;
			}
			
			SendScanPhase(results, location, UnityEventScanPhase.ArgumentType, objectArgumentAssemblyTypeName);

			var mode = call.FindPropertyRelative("m_Mode");
			if (mode == null || mode.propertyType != SerializedPropertyType.Enum)
			{
				Debug.LogError(Maintainer.ErrorForSupport(
					"Couldn't find m_Mode in serialized UnityEvent's call!"));
				return false;
			}
			
			SendScanPhase(results, location, UnityEventScanPhase.CallMode, mode);

			var modeValue = (PersistentListenerMode)mode.enumValueIndex;

			var dummyEvent = CSReflectionTools.GetDummyEvent(location.Property);
			if (dummyEvent == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Couldn't get something from GetDummyEvent!",
					IssuesFinder.ModuleName));
				return false;
			}

			var type = CSReflectionTools.objectType;
			var stringValue = objectArgumentAssemblyTypeName.stringValue;

			if (!string.IsNullOrEmpty(stringValue))
				type = Type.GetType(stringValue, false) ?? typeof(Object);

			if (!UnityEventDrawer.IsPersistantListenerValid(dummyEvent, methodNameValue, callTargetObject, modeValue,
				type))
			{
				var narrow = location.Narrow();
				narrow.PropertyOverride(methodName.propertyPath);
				SendScanPhase(results, narrow, UnityEventScanPhase.InvalidListener);
			}

			return true;
		}

		private void SendScanPhase(T results, PropertyLocation location, UnityEventScanPhase phase,
			SerializedProperty newProperty = null)
		{
			NarrowLocation narrow = null;
			
			if (newProperty != null)
			{
				narrow = location.Narrow();
				narrow.PropertyOverride(newProperty);
			}

			foreach (var listener in ScanListeners)
			{
				listener.UnityEventProperty(results, narrow != null ? narrow : location, phase);
			}
		}
	}
}