#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.References.Entry
{
	using Core;
	using Tools;
	using Settings;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	internal static class GenericObjectProcessor
	{
		private const string MainTexturePropertyName = "_MainTex";
		private static readonly int MainTextureShaderProperty = Shader.PropertyToID(MainTexturePropertyName);

		public static void ProcessObject(Location currentLocation, Object inspectedUnityObject, Object target, EntryAddSettings addSettings, ProcessObjectReferenceHandler processReferenceCallback)
		{
			var deepHierarchySearch = EntryFinder.currentScope == EntryFinderScope.Hierarchy &&
			                          UserSettings.References.DeepHierarchySearch;
			
			var deepProjectSearch = EntryFinder.currentScope == EntryFinderScope.Project && UserSettings.References.DeepProjectSearch;
			var deepSearch = currentLocation == Location.ScriptAsset || deepHierarchySearch || deepProjectSearch;
			var componentTraverseInfo = new SerializedObjectTraverseInfo(target, !deepSearch);

			string lastScriptPropertyName = null;

			CSTraverseTools.TraverseObjectProperties(componentTraverseInfo, (info, sp) =>
			{
				if (currentLocation == Location.ScriptAsset)
					lastScriptPropertyName = CSTraverseTools.TryGetMonoScriptDefaultPropertyName(sp);

				if (sp.propertyType == SerializedPropertyType.ObjectReference && sp.objectReferenceValue != null)
				{
					// skipping components reference to their game object
					if (deepHierarchySearch && sp.propertyPath == "m_GameObject")
						return;
					
					string propertyName;

					if (lastScriptPropertyName != null)
					{
						propertyName = lastScriptPropertyName;
						lastScriptPropertyName = string.Empty;
					}
					else
					{
						propertyName = sp.propertyPath;
					}

					/*if (string.Equals(propertyName, "m_Script", StringComparison.OrdinalIgnoreCase))
					{
						propertyName = "Script source";
					}*/
					
					addSettings.propertyPath = propertyName;
					processReferenceCallback(inspectedUnityObject, sp.objectReferenceInstanceIDValue, addSettings);

					/* material instance handling */

					var material = sp.objectReferenceValue as Material;
					if (material == null) return;

					if (currentLocation == Location.PrefabAssetGameObject)
					{
						if (AssetDatabase.GetAssetPath(material) != AssetDatabase.GetAssetPath(target)) return;
						if (AssetDatabase.IsSubAsset(material)) return;
					}
					else
					{
						if (AssetDatabase.Contains(material)) return;
					}

					addSettings.prefix = "[Material Instance]";
					addSettings.suffix = "(Main Texture)";

					var mainTextureInstanceId = 0;
					if (material.HasProperty(MainTextureShaderProperty))
					{
						var mainTexture = material.mainTexture;
						mainTextureInstanceId = mainTexture != null ? mainTexture.GetInstanceID() : 0;
					}

					processReferenceCallback(inspectedUnityObject, mainTextureInstanceId, addSettings);

					addSettings.suffix = "(Shader)";

					var shaderInstanceId = material.shader != null ? material.shader.GetInstanceID() : 0;
					processReferenceCallback(inspectedUnityObject, shaderInstanceId, addSettings);

					CSTraverseTools.TraverseMaterialTexEnvs(new SerializedObjectTraverseInfo(material),
						(traverseInfo, texEnv, texEnvName, texEnvTexture) =>
						{
							if (texEnvName == MainTexturePropertyName)
								return;

							if (texEnvTexture == null)
								return;

							if (texEnvTexture.propertyType != SerializedPropertyType.ObjectReference)
								return;

							addSettings.suffix = " (" + texEnvName + ")";
							processReferenceCallback(inspectedUnityObject, texEnvTexture.objectReferenceInstanceIDValue, addSettings);
						});
				}

				lastScriptPropertyName = null;
			});
		}
	}
}