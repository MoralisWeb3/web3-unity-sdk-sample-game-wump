#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	/// <summary>
	/// LocationGroup of the record-based results item.
	/// </summary>
	public enum LocationGroup : byte
	{
		Unknown = 0,
		Scene = 5,
		Asset = 7,
		PrefabAsset = 10
	}
}