#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues.Detectors
{
	using Core.Extension;
	using Settings;

	internal abstract class IssueDetector : MaintainerExtension, IIssueDetector
	{
		public abstract DetectorInfo Info { get; }
		
		protected override bool Enabled
		{
			get
			{
				return this.GetEnabled();
			}

			set
			{
				this.SetEnabled(value);
			}
		}
	}
	
	internal static class IssueDetectorExtensions
	{
		public static bool GetEnabled<T>(this T instance) where T : IIssueDetector
		{
			return ProjectSettings.Issues.GetDetectorEnabled(instance);
		}
		
		public static void SetEnabled<T>(this T instance, bool enabled) where T : IIssueDetector
		{
			ProjectSettings.Issues.SetDetectorEnabled(instance, enabled);
		}
	}
}