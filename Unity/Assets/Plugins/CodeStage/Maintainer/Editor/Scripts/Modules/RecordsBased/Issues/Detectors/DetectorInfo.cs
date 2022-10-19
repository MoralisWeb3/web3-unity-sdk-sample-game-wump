#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using UnityEngine;

	internal struct DetectorInfo
	{
		public IssueGroup Group { get; private set; }
		public DetectorKind Kind { get; private set; }
		public IssueSeverity Severity { get; private set; }
		public string Name { get; private set; }
		public string Tooltip { get; private set; }
		
		private DetectorInfo(IssueGroup group, DetectorKind kind, IssueSeverity severity, string name, string tooltip = null) : this()
		{
			Group = group;
			Kind = kind;
			Severity = severity;
			Name = name;
			Tooltip = tooltip;
		}

		public GUIContent GetGUIContent()
		{
			return new GUIContent(Name, Tooltip);
		}

		public static DetectorInfo From(IssueGroup group, DetectorKind kind, IssueSeverity severity, string name, string tooltip = null)
		{
			return new DetectorInfo(group, kind, severity, name, tooltip);
		}
	}
}