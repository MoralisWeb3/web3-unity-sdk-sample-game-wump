#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface ISceneBeginScanListener<T> where T : IScanListenerResults
	{
		void SceneBegin(T results, AssetLocation location);
	}
}