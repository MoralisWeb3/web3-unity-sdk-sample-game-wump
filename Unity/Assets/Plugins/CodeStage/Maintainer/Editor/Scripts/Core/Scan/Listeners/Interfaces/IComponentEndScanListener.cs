#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface IComponentEndScanListener<T> where T : IScanListenerResults
	{
		void ComponentEnd(T results, ComponentLocation location);
	}
}