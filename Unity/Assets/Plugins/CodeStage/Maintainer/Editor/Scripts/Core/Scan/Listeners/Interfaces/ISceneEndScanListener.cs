#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface ISceneEndScanListener<T> where T : IScanListenerResults
	{
		void SceneEnd(T results, AssetLocation location);
	}
}