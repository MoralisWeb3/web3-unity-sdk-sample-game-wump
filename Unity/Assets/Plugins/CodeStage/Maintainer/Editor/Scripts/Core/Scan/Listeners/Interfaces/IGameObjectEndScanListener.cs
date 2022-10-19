#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface IGameObjectEndScanListener<T> where T : IScanListenerResults
	{
		void GameObjectEnd(T results, GameObjectLocation location);
	}
}