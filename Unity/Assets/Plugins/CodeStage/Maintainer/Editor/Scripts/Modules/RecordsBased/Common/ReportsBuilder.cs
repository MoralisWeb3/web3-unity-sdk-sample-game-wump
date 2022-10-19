#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer
{
	using System;
	using System.Text;
	using UnityEditorInternal;
	using UnityEngine;

	/// <summary>
	/// Allows building reports for the Maintainer modules.
	/// </summary>
	public static class ReportsBuilder
	{
		private static readonly StringBuilder reportStringBuilder = new StringBuilder();

		/// <summary>
		/// Iterates over array of specified records and generates report.
		/// </summary>
		/// <param name="module">Module name.</param>
		/// <param name="records">Array of records to iterate.</param>
		/// <param name="optionalHeader">Optional string to add as a report header.</param>
		/// <param name="optionalFooter">Optional string to add as a report footer.</param>
		/// <returns>Final report string.</returns>
		public static string GenerateReport<T>(string module, T[] records, string optionalHeader = null, string optionalFooter = null) where T:RecordBase
		{
			reportStringBuilder.Length = 0;

			string isPro = null;
			if (Application.HasProLicense())
			{
				isPro = " Plus / Pro";
			}

			reportStringBuilder.
				AppendLine("////////////////////////////////////////////////////////////////////////////////").
				Append("// ").Append(module).AppendLine(" report").
				Append("// ").AppendLine(Application.dataPath.Remove(Application.dataPath.LastIndexOf("/", StringComparison.Ordinal), 7)).
				AppendLine("////////////////////////////////////////////////////////////////////////////////").
				Append("// Maintainer ").AppendLine(Maintainer.Version).
				Append("// Unity ").Append(InternalEditorUtility.GetFullUnityVersion()).AppendLine(isPro).
				AppendLine("//").
				Append("// Homepage: ").AppendLine(Maintainer.HomePage).
				Append("// Contacts: ").AppendLine(Maintainer.SupportContact).
				AppendLine("////////////////////////////////////////////////////////////////////////////////");

			if (records != null && records.Length > 0)
			{
				reportStringBuilder.Append("Items found: ").AppendLine(records.Length.ToString());
				
				if (!string.IsNullOrEmpty(optionalHeader))
				{
					reportStringBuilder.AppendLine(optionalHeader);
				}

				foreach (var record in records)
				{
					reportStringBuilder.AppendLine("---").AppendLine(record.ToString(true));
				}

				if (!string.IsNullOrEmpty(optionalFooter))
				{
					reportStringBuilder.AppendLine("---").AppendLine(optionalFooter);
				}
			}
			else
			{
				reportStringBuilder.AppendLine("No records to report.");
			}

			return reportStringBuilder.ToString();
		}
	}
}