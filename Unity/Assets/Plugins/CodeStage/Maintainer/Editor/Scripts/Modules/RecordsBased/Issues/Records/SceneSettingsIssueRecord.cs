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
	using Object = UnityEngine.Object;

	public enum SceneSettingsKind
	{
		LightmapSettings = 5,
		RenderSettings = 7
	}

	[Serializable]
	public class SceneSettingsIssueRecord : AssetIssueRecord, IShowableRecord
	{
		public string PropertyPath { get; private set; }
		public SceneSettingsKind SettingsKind { get; private set; }

		public override bool IsFixable
		{
			get
			{
				return Kind == IssueKind.MissingReference;;
			}
		}

		public void Show()
		{
			var openResult = CSSelectionTools.OpenSceneForReveal(Path);
			if (openResult.success)
			{
				var instanceId = CSAssetTools.GetMainAssetInstanceID(Path);
				EditorGUIUtility.PingObject(instanceId);
			}

			switch (SettingsKind)
			{
				case SceneSettingsKind.RenderSettings:
				case SceneSettingsKind.LightmapSettings:
					CSMenuTools.ShowSceneSettingsLighting();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal static SceneSettingsIssueRecord Create(IIssueDetector detector, SceneSettingsKind settingsKind, IssueKind issueKind, PropertyLocation location)
		{
			return new SceneSettingsIssueRecord(detector, settingsKind, issueKind, location);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			return false;
		}

		internal SceneSettingsIssueRecord(IIssueDetector detector, SceneSettingsKind settingsKind, IssueKind issueKind, PropertyLocation location) : base(detector, issueKind, location)
		{
			SettingsKind = settingsKind;
			PropertyPath = location.PropertyPath;
		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.AppendLine("<b>Scene: </b>" + CSPathTools.NicifyAssetPath(Path, true));
			text.Append("<b>Settings: </b>" + SettingsKind);
			if (!string.IsNullOrEmpty(PropertyPath))
			{
				var propertyName = CSObjectTools.GetNicePropertyPath(PropertyPath);
				text.Append("\n<b>Property:</b> ").Append(propertyName);
			}
		}

		internal override FixResult PerformFix(bool batchMode)
		{
			CSSceneTools.OpenSceneResult openSceneResult = null;

			if (!batchMode)
			{
				openSceneResult = CSSceneTools.OpenScene(Path);
				if (!openSceneResult.success)
				{
					return FixResult.CreateError("Couldn't open scene");
				}
			}

			FixResult result;

			var settingsObject = GetSettingsObjectWithThisIssue();
			if (settingsObject == null)
			{
				result = new FixResult(false);
				if (batchMode)
				{
					Debug.LogWarning(Maintainer.ConstructLog("Couldn't find " + SettingsKind + " object for issue:\n" + this, IssuesFinder.ModuleName));
				}
				else
				{
					result.SetErrorText("Couldn't find " + SettingsKind + " object at\n" + Path);
				}
				return result;
			}

			result = IssuesFixer.FixMissingReference(settingsObject, PropertyPath, LocationGroup.Scene);

			if (!batchMode)
			{
				CSSceneTools.SaveScene(openSceneResult.scene);
				CSSceneTools.CloseOpenedSceneIfNeeded(openSceneResult);
			}

			return result;
		}

		private Object GetSettingsObjectWithThisIssue()
		{
			Object result;

			switch (SettingsKind)
			{
				case SceneSettingsKind.LightmapSettings:
					result = CSSettingsTools.GetInSceneLightmapSettings();
					break;
				case SceneSettingsKind.RenderSettings:
					result = CSSettingsTools.GetInSceneRenderSettings();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}
	}
}