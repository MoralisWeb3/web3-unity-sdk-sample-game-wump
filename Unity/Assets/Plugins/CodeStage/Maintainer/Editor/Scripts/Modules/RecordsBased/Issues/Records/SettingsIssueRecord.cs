#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Issues
{
	using System;
	using System.Text;
	using Core;
	using Core.Scan;
	using Detectors;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class SettingsIssueRecord : AssetIssueRecord, IShowableRecord
	{
		public string PropertyPath { get; private set; }
		internal AssetSettingsKind SettingsKind { get; private set; }

		public override bool IsFixable
		{
			get
			{
				return Kind == IssueKind.MissingReference;
			}
		}

		public void Show()
		{
			CSEditorTools.RevealInSettings(SettingsKind);
		}

		internal static SettingsIssueRecord Create(IIssueDetector detector, AssetSettingsKind settingsKind, IssueKind type, PropertyLocation location, string body = null)
		{
			return new SettingsIssueRecord(detector, settingsKind, type, location, body);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			return false;
		}

		internal SettingsIssueRecord(IIssueDetector detector, AssetSettingsKind settingsKind, IssueKind kind, PropertyLocation location, string body):base(detector, kind, location)
		{
			SettingsKind = settingsKind;
			PropertyPath = location.PropertyPath;
			BodyPostfix = body;
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.Append("<b>Settings: </b>" + SettingsKind);
			if (!string.IsNullOrEmpty(PropertyPath))
			{
				var propertyName = CSObjectTools.GetNicePropertyPath(PropertyPath);
				text.Append("\n<b>Property:</b> ").Append(propertyName);
			}
		}

		internal override FixResult PerformFix(bool batchMode)
		{
			FixResult result;
			var assetObject = AssetDatabase.LoadMainAssetAtPath(Path);

			// workaround for Unity 5.6 issue: LoadMainAssetAtPath returns null for settings assets
			if (assetObject == null)
			{
				var allObjects = AssetDatabase.LoadAllAssetsAtPath(Path);
				if (allObjects != null && allObjects.Length > 0)
				{
					assetObject = allObjects[0];
				}
			}

			if (assetObject == null)
			{
				result = new FixResult(false);
				if (batchMode)
				{
					Debug.LogWarning(Maintainer.ConstructLog("Couldn't find settings asset for issue:\n" + this, IssuesFinder.ModuleName));
				}
				else
				{
					result.SetErrorText("Couldn't find settings asset at\n" + Path);
				}
				return result;
			}

			result = IssuesFixer.FixMissingReference(assetObject, PropertyPath, LocationGroup.Asset);
			return result;
		}
	}
}