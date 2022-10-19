#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{

	internal interface IPropertyScanListener
	{
		
	}
	
	internal interface IGenericPropertyScanListener<T> : IPropertyScanListener where T : IScanListenerResults
	{
		PropertyScanDepth GetPropertyScanDepth(ComponentLocation location);
		void Property(T results, PropertyLocation location);
	}
	
	internal interface IUnityEventScanListener<T> : IPropertyScanListener where T : IScanListenerResults
	{
		void UnityEventProperty(T results, PropertyLocation location, UnityEventScanPhase phase);
	}
}