#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Routines
{
	using Core.Scan;
	using Detectors;

/*#if !UNITY_2019_2_OR_NEWER
	[UnityEditor.InitializeOnLoad]
#endif*/
	internal class UnityEventIssuesScanner : UnityEventScanner<DetectorResults>
	{
/*#if !UNITY_2019_2_OR_NEWER
		static UnityEventIssuesScanner()
		{
			IssuesFinderDetectors.AddInternalPropertyScanner(new UnityEventIssuesScanner());
		}
#endif*/
	}
}