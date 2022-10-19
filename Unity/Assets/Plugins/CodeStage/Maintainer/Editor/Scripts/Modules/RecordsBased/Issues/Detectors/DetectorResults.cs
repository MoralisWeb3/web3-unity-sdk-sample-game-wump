#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using System.Collections.Generic;
	using Core.Scan;
	
	internal class DetectorResults : ScanListenerResults<IssueRecord>
	{
		internal void Set(ref List<IssueRecord> overrideResults)
		{
			results = overrideResults;
		}
	}
}