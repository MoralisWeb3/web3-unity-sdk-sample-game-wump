#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System;
	using Core.Scan;

	internal interface IComponentEndIssueDetector : IComponentEndScanListener<DetectorResults>
	{
		// return null to check all types
		// checked using Type.IsAssignableFrom() API
		Type[] ComponentTypes { get; }
	}
}