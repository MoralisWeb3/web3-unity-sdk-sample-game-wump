#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

#if UNITY_2019_1_OR_NEWER
namespace CodeStage.Maintainer.Issues
{
	using System;
	using System.Text;
	using Core;
	using Core.Scan;
	using Detectors;
	using Tools;
	using UI;

	[Serializable]
	internal class ShaderIssueRecord : AssetIssueRecord, IShowableRecord
	{
		public void Show()
		{
			if (!CSSelectionTools.RevealAndSelectFileAsset(Path))
			{
				MaintainerWindow.ShowNotification("Can't show it properly");
			}
		}

		internal static ShaderIssueRecord Create(IIssueDetector detector, AssetLocation location)
		{
			return new ShaderIssueRecord(detector, location);
		}

		internal override bool MatchesFilter(FilterItem newFilter)
		{
			return false;
		}

		public override bool IsFixable
		{
			get
			{
				return false;
			}
		}

		protected ShaderIssueRecord(IIssueDetector detector, AssetLocation location):base(detector, IssueKind.ShaderError, location)
		{

		}

		protected override void ConstructBody(StringBuilder text)
		{
			text.Append("<b>Shader:</b> ");
			text.Append(CSPathTools.NicifyAssetPath(Path, true));
		}
	}
}
#endif