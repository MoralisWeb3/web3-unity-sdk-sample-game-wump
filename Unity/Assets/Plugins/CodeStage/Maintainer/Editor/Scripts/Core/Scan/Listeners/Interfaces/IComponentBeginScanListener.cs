#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface IComponentBeginScanListener<T> where T : IScanListenerResults
	{
		void ComponentBegin(T results, ComponentLocation location);
	}
}