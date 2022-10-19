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
	using UI;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class UnityObjectAssetIssueRecord : AssetIssueRecord, IShowableRecord
	{
		public string propertyPath;
		public string typeName;

		[SerializeField]
		private bool missingEventMethod;

		public override bool IsFixable
		{
			get
			{
				return Kind == IssueKind.MissingReference && !missingEventMethod;
			}
		}

		public void Show()
		{
			if (!CSSelectionTools.RevealAndSelectFileAsset(Path))
			{
				MaintainerWindow.ShowNotification("Can't show it properly");
			}
		}

		internal static UnityObjectAssetIssueRecord Create(IIssueDetector detector, IssueKind type, AssetLocation location)
		{
			return new UnityObjectAssetIssueRecord(detector, type, location);
		}

		internal static UnityObjectAssetIssueRecord Create(IIssueDetector detector, IssueKind type, ComponentLocation location)
		{
			return new UnityObjectAssetIssueRecord(detector, type, location);
		}

		internal static UnityObjectAssetIssueRecord Create(IIssueDetector detector, IssueKind type, PropertyLocation location)
		{
			return new UnityObjectAssetIssueRecord(detector, type, location);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			var filters = new[] { newFilter };

			switch (newFilter.kind)
			{
				case FilterKind.Path:
				case FilterKind.Directory:
				case FilterKind.FileName:
				case FilterKind.Extension:
					return !string.IsNullOrEmpty(Path) && CSFilterTools.IsValueMatchesAnyFilterOfKind(Path, filters, newFilter.kind);
				case FilterKind.Type:
				{
					return !string.IsNullOrEmpty(typeName) && CSFilterTools.IsValueMatchesAnyFilterOfKind(typeName, filters, newFilter.kind);
				}
				case FilterKind.NotSet:
					return false;
				default:
					Debug.LogWarning(Maintainer.ErrorForSupport("Unknown filter kind: " + newFilter.kind, IssuesFinder.ModuleName));
					return false;
			}
		}

		internal UnityObjectAssetIssueRecord(IIssueDetector detector, IssueKind kind, AssetLocation location) : base(detector, kind, location)
		{

		}

		internal UnityObjectAssetIssueRecord(IIssueDetector detector, IssueKind kind, ComponentLocation location) : this(detector, kind, location as AssetLocation)
		{
			typeName = location.ComponentName;
		}

		internal UnityObjectAssetIssueRecord(IIssueDetector detector, IssueKind kind, PropertyLocation location) : this(detector, kind, location as ComponentLocation)
		{
			propertyPath = location.PropertyPath;

			if (propertyPath.EndsWith("].m_MethodName", StringComparison.OrdinalIgnoreCase))
			{
				missingEventMethod = true;
			}
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.Append("<b>Asset:</b> ");
			text.Append(CSPathTools.NicifyAssetPath(Path, true));

			if (!string.IsNullOrEmpty(typeName))
			{
				text.Append("\n<b>Type:</b> ").Append(typeName);
			}

			if (!string.IsNullOrEmpty(propertyPath))
			{
				var propertyName = CSObjectTools.GetNicePropertyPath(propertyPath);
				text.Append("\n<b>Property:</b> ").Append(propertyName);
			}
		}

		internal override FixResult PerformFix(bool batchMode)
		{
			FixResult result;
			var unityObject = AssetDatabase.LoadMainAssetAtPath(Path);

			if (unityObject == null)
			{
				result = new FixResult(false);
				if (batchMode)
				{
					Debug.LogWarning(Maintainer.ConstructLog("Can't find Unity Object for issue:\n" + this, IssuesFinder.ModuleName));
				}
				else
				{
					result.SetErrorText("Couldn't find Unity Object\n" + Path);
				}
				return result;
			}

			result = IssuesFixer.FixMissingReference(unityObject, propertyPath, LocationGroup.Asset);
			return result;
		}
	}
}