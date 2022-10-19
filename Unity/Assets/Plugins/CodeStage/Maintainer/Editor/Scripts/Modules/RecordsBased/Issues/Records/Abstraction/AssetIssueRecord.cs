#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System;
	using Core.Scan;
	using Detectors;

	[Serializable]
	public abstract class AssetIssueRecord : IssueRecord
	{
		public string Path { get; private set; }

		internal AssetIssueRecord(IIssueDetector detector, IssueKind kind, AssetLocation location) : base(detector, kind, location)
		{
			Path = location.Asset.Path;
		}
	}
}
