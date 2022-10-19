#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System;
	using System.Text;
	using Core.Scan;
	using Detectors;

	/// <summary>
	/// Result of the issue fix operation.
	/// </summary>
	[Serializable]
	public class FixResult
	{
		/// <summary>
		/// Returns true if fix was successful and confirmed, false otherwise.
		/// </summary>
		public bool Success { get; private set; }

		/// <summary>
		/// Contains error text in case fix was not successful. May be empty if fail cause is not known.
		/// </summary>
		public string ErrorText { get; private set; }

		internal FixResult(bool success, string errorText = null)
		{
			Success = success;
			ErrorText = errorText;
		}

		internal static FixResult CreateError(string errorText)
		{
			return new FixResult(false, errorText);
		}

		internal void SetErrorText(string errorText)
		{
			ErrorText = errorText;
		}
	}

	/// <summary>
	/// Base class for all Issues Finder results items.
	/// </summary>
	[Serializable]
	public abstract class IssueRecord: RecordBase, IScanListenerResult
	{
		/// <summary>
		/// Describes found issue's kind.
		/// </summary>
		public IssueKind Kind { get; private set; }

		/// <summary>
		/// Describes found issue's severity.
		/// </summary>
		public IssueSeverity Severity { get; private set; }
		
		internal string Header { get; set; }
		
		internal FixResult fixResult;

		/// <summary>
		/// Perform fix attempt. Call only if #IsFixable returns true.
		/// </summary>
		/// <param name="batchMode">Pass true when fixing more than 1 issue at a time to improve fixing performance using batch approach.</param>
		/// <returns>Fixing attempt result.</returns>
		public FixResult Fix(bool batchMode)
		{
			fixResult = PerformFix(batchMode);
			return fixResult;
		}

		/// <summary>
		/// Returns true if current issue type is potentially fixable, returns false otherwise.
		/// </summary>
		public abstract bool IsFixable { get; }

		// -----------------------------------------------------------------------------
		// base constructors
		// -----------------------------------------------------------------------------

		internal IssueRecord(IIssueDetector detector, IssueKind kind, Location location):base(location.Group)
		{
			Kind = kind;
			Severity = detector.Info.Severity;
			Header = detector.Info.Name;
		}

		// -----------------------------------------------------------------------------
		// issue compact line generation
		// -----------------------------------------------------------------------------

		protected override void ConstructCompactLine(StringBuilder text)
		{
			ConstructHeader(text);
		}

		// -----------------------------------------------------------------------------
		// issue header generation
		// -----------------------------------------------------------------------------

		protected override void ConstructHeader(StringBuilder text)
		{
			text.Append(Header);
		}

		internal virtual FixResult PerformFix(bool batchMode)
		{
			return null;
		}
	}
}