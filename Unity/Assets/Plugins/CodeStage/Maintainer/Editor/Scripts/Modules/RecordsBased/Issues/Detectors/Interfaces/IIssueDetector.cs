#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using Core.Extension;

	internal interface IIssueDetector : IMaintainerExtension
	{
		DetectorInfo Info { get; }
	}
}