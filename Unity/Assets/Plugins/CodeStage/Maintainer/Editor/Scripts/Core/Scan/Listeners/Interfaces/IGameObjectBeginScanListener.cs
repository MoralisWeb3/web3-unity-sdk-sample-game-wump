#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	internal interface IGameObjectBeginScanListener<T> where T : IScanListenerResults
	{
		void GameObjectBegin(T results, GameObjectLocation location);
	}
}