#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using EditorCommon.Tools;
	using UnityEditor;
	using UnityEngine;

	internal static class CSIcons
	{
		public static Texture ArrowLeft { get { return CSTextureLoader.GetIconTexture("ArrowLeft.png"); } }
		public static Texture ArrowRight { get { return CSTextureLoader.GetIconTexture("ArrowRight.png"); } }
		public static Texture AutoFix { get { return CSTextureLoader.GetIconTexture("AutoFix.png"); } }
		public static Texture Check { get { return CSTextureLoader.GetIconTexture("Check.png"); } }
		public static Texture Clean { get { return CSTextureLoader.GetIconTexture("Clean.png"); } }
		public static Texture Clear { get { return CSTextureLoader.GetIconTexture("Clear.png"); } }
		public static Texture Collapse { get { return CSTextureLoader.GetIconTexture("Collapse.png"); } }
		public static Texture Copy { get { return CSTextureLoader.GetIconTexture("Copy.png"); } }
		public static Texture Delete { get { return CSTextureLoader.GetIconTexture("Delete.png"); } }
		public static Texture DoubleArrowLeft { get { return CSTextureLoader.GetIconTexture("DoubleArrowLeft.png"); } }
		public static Texture DoubleArrowRight { get { return CSTextureLoader.GetIconTexture("DoubleArrowRight.png"); } }
		public static Texture Edit { get { return CSTextureLoader.GetIconTexture("Edit.png"); } }
		public static Texture Expand { get { return CSTextureLoader.GetIconTexture("Expand.png"); } }
		public static Texture Export { get { return CSTextureLoader.GetIconTexture("Export.png"); } }
		public static Texture Filter { get { return CSTextureLoader.GetIconTexture("Filter.png"); } }
		public static Texture Gear { get { return CSTextureLoader.GetIconTexture("Gear.png"); } }
		public static Texture Hide { get { return CSTextureLoader.GetIconTexture("Hide.png"); } }
		public static Texture Home { get { return CSTextureLoader.GetIconTexture("Home.png"); } }
		public static Texture Issue { get { return CSTextureLoader.GetIconTexture("Issue.png"); } }
		public static Texture Log { get { return CSTextureLoader.GetIconTexture("Log.png"); } }
		public static Texture Maintainer { get { return CSTextureLoader.GetIconTexture("Maintainer.png"); } }
		public static Texture Minus { get { return CSTextureLoader.GetIconTexture("Minus.png"); } }
		public static Texture More { get { return CSTextureLoader.GetIconTexture("More.png"); } }
		public static Texture Plus { get { return CSTextureLoader.GetIconTexture("Plus.png"); } }
		public static Texture Publisher { get { return CSTextureLoader.GetIconTexture("Publisher.png"); } }
		public static Texture Restore { get { return CSTextureLoader.GetIconTexture("Restore.png"); } }
		public static Texture Reveal { get { return CSTextureLoader.GetIconTexture("Reveal.png"); } }
		public static Texture RevealBig { get { return CSTextureLoader.GetIconTexture("RevealBig.png"); } }
		public static Texture SelectAll { get { return CSTextureLoader.GetIconTexture("SelectAll.png"); } }
		public static Texture SelectNone { get { return CSTextureLoader.GetIconTexture("SelectNone.png"); } }
		public static Texture Show { get { return CSTextureLoader.GetIconTexture("Show.png"); } }
		public static Texture Support { get { return CSTextureLoader.GetIconTexture("Support.png"); } }
		public static Texture Discord{ get { return CSTextureLoader.GetIconTexture("Discord.png"); } }
		public static Texture Repeat { get { return CSTextureLoader.GetIconTexture("Repeat.png"); } }
	}
	
	internal static class CSImages
	{
		public static Texture Logo { get { return CSTextureLoader.GetTexture("Logo.png"); } }
	}
}