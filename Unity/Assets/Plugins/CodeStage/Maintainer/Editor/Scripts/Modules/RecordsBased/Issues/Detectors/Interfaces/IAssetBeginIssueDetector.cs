#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core.Scan;

	internal interface IAssetBeginIssueDetector : IAssetBeginScanListener<DetectorResults>
	{
		// return null to check all types
		// checked using Type.IsAssignableFrom() API
		Type[] AssetTypes { get; }
	}
}