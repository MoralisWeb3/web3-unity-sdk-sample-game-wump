#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using System.Diagnostics;
	using System.Globalization;
	using Core;
	using EditorCommon.Tools;
	using Settings;
	using Tools;

	using UnityEditor;
	using UnityEngine;
	using Debug = UnityEngine.Debug;

	internal class AboutTab : BaseTab
	{
		private const string UasLink = "https://assetstore.unity.com/packages/slug/32199?aid=1011lGBp&pubref=Maintainer";
		private const string UasReviewLink = UasLink + "#reviews";
		private const string UasProfileLink = "https://assetstore.unity.com/publishers/3918?aid=1011lGBp&pubref=Maintainer";
		private const string Homepage = "https://codestage.net/uas/maintainer";
		private const string DiscordLink = "https://discord.gg/FRK5HRzZvq";
		private const string SupportLink = "https://codestage.net/contacts/";
		private const string ChangelogLink = "https://codestage.net/uas_files/maintainer/CHANGELOG.md";

		private bool showDebug = false;

		protected override string CaptionName
		{
			get { return "About"; }
		}

		protected override Texture CaptionIcon
		{
			get { return CSEditorIcons.Help; }
		}

		public AboutTab(MaintainerWindow window) : base(window) {}

		public void Draw()
		{
			using (new GUILayout.HorizontalScope())
			{
				DrawLogo();
				DrawButtons();
			}
		}

		private void DrawLogo()
		{
			using (new GUILayout.VerticalScope(UIHelpers.panelWithBackground, GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)))
			{
				GUILayout.FlexibleSpace();

				using (new GUILayout.HorizontalScope())
				{
					GUILayout.FlexibleSpace();

					var logo = CSImages.Logo;
					if (logo != null)
					{
						logo.wrapMode = TextureWrapMode.Clamp;
						var logoRect = EditorGUILayout.GetControlRect(GUILayout.Width(logo.width),
							GUILayout.Height(logo.height));
						GUI.DrawTexture(logoRect, logo);
						GUILayout.Space(5);
					}

					GUILayout.FlexibleSpace();
				}

				GUILayout.FlexibleSpace();
			}
		}
		
		private void DrawButtons()
		{
			using (new GUILayout.HorizontalScope(UIHelpers.panelWithBackground, GUILayout.ExpandHeight(true),
					GUILayout.ExpandWidth(true)))
				{
					GUILayout.Space(10);

					using (new GUILayout.VerticalScope())
					{
						GUILayout.Space(10);

						DrawCommonButtons();

						GUILayout.Space(10);

						//GUILayout.Space(10);
						//GUILayout.Label("Asset Store links", UIHelpers.centeredLabel);
						UIHelpers.Separator();
						GUILayout.Space(10);

						DrawAssetStoreButtons();

						DrawDebugButtons();
					}
					GUILayout.Space(10);
				}
		}

		private void DrawCommonButtons()
		{
			GUILayout.Label("<size=18>Maintainer</size>",
				UIHelpers.centeredLabel);
			GUILayout.Label("<b>Version " + Maintainer.Version + "</b>",
				UIHelpers.centeredLabel);
			GUILayout.Space(10);
			GUILayout.Label("Developed by Dmitriy Yukhanov\n" +
							"Logo by Daniele Giardini\n" +
							"Icons by Google, Austin Andrews, Cody", UIHelpers.centeredLabel);
			GUILayout.Space(10);
			UIHelpers.Separator();
			GUILayout.Space(10);
			if (UIHelpers.ImageButton("Homepage", CSIcons.Home))
			{
				Application.OpenURL(Homepage);
			}
			
			GUILayout.Space(10);

			using (new GUILayout.HorizontalScope())
			{
				if (UIHelpers.ImageButton("Discord", CSIcons.Discord))
				{
					Application.OpenURL(DiscordLink);
				}
				GUILayout.Space(5);
				if (UIHelpers.ImageButton("Support", CSIcons.Support))
				{
					Application.OpenURL(SupportLink);
				}
			}

			GUILayout.Space(10);
			if (UIHelpers.ImageButton("Full changelog (online)", CSIcons.Log))
			{
				Application.OpenURL(ChangelogLink);
			}
		}
		
		private void DrawAssetStoreButtons()
		{
			if (UIHelpers.ImageButton("Code Stage at the Asset Store", CSIcons.Publisher))
			{
				Application.OpenURL(UasProfileLink);
			}
			GUILayout.Space(10);
			if (UIHelpers.ImageButton("Maintainer at the Asset Store", CSIcons.Maintainer))
			{
				Application.OpenURL(UasLink);
			}
			GUILayout.Space(10);
			if (UIHelpers.ImageButton("Leave a review", CSEditorIcons.FavoriteIcon))
			{
				Application.OpenURL(UasReviewLink);
			}

			GUILayout.Label(
				"It's really important to know your opinion,\nreviews are <b>greatly appreciated!</b>",
				UIHelpers.centeredLabel);
			GUILayout.Space(10);
		}
		
		private void DrawDebugButtons()
		{
			if (Event.current.isKey && Event.current.type == EventType.KeyDown && Event.current.control && Event.current.keyCode == KeyCode.D)
			{
				showDebug = !showDebug;
				Event.current.Use();
			}

			if (showDebug)
			{
				GUILayout.Space(5);
				UIHelpers.Separator();
				GUILayout.Space(5);
				GUILayout.Label("Welcome to secret debug mode =D");
				if (GUILayout.Button("Remove Assets Map"))
				{
					AssetsMap.Delete();
				}

				if (GUILayout.Button("Measure Assets Map build time"))
				{
					var sw = Stopwatch.StartNew();
					var canceled = false;
					var map = AssetsMap.CreateNew(out canceled);
					sw.Stop();
					Debug.Log(Maintainer.ConstructLog("Asset Map build took " +
													  sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) +
													  " seconds, assets found: " + map.assets.Count));
				}
							
				if (GUILayout.Button("Remove User Settings and Close"))
				{
					window.Close();
					UserSettings.Delete();
				}

				if (GUILayout.Button("Remove All Settings and Close"))
				{
					window.Close();
					ProjectSettings.Delete();
				}

				if (GUILayout.Button("Re-save all scenes in project"))
				{
					CSSceneTools.ReSaveAllScenes();
				}
				
				if (GUILayout.Button("Reload settings"))
				{
					ProjectSettings.Reload();
				}
			}
		}
	}
}