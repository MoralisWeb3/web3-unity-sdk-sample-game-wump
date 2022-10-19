#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface IAssetBeginScanListener<T> where T : IScanListenerResults
	{
		void AssetBegin(T results, AssetLocation location);
	}
}