#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface IAssetEndScanListener<T> where T : IScanListenerResults
	{
		void AssetEnd(T results, AssetLocation location);
	}
}