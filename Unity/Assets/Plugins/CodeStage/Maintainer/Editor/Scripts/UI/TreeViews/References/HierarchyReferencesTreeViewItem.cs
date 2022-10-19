#region copyright
// ---------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// ---------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.UI
{
	using EditorCommon.Tools;
	using References;
	using Tools;
	using UnityEditor;
	using UnityEngine;

	internal class HierarchyReferencesTreeViewItem<T> : MaintainerTreeViewItem<T> where T : HierarchyReferenceItem
	{
		public HierarchyReferencesTreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName, data) { }

		protected override void Initialize()
		{
			/*if (depth != 0)
			{
				return;
			}*/

			if (data.reference == null || data.reference.objectInstanceId == 0)
			{
				icon = (Texture2D)CSEditorIcons.WarnSmall;
				return;
			}
			
			var objectInstance = EditorUtility.InstanceIDToObject(data.reference.objectInstanceId);
			if (objectInstance == null)
			{
				icon = (Texture2D)CSEditorIcons.WarnSmall;
				return;
			}

			if (data.reference.componentId >= 0)
			{
				icon = (Texture2D)CSEditorIcons.Script;
				
				if (objectInstance is GameObject)
				{
					var component = CSComponentTools.GetComponentWithIndex(objectInstance as GameObject, data.reference.componentId);
					if (component != null)
					{
						var texture = CSTextureLoader.GetTypeImage(component.GetType());
						if (texture != null && texture is Texture2D)
						{
							icon = (Texture2D)texture;
						}
						else
						{
							icon = (Texture2D)CSEditorIcons.Script;
						}
					}
				}
			}
			else
			{
				icon = (Texture2D)CSEditorIcons.GameObject;
			}
		}
	}
}