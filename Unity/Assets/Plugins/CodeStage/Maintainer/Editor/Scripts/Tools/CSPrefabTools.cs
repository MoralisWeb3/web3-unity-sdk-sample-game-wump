#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	
	using UnityEngine;
	using Object = UnityEngine.Object;
	
#if UNITY_2021_2_OR_NEWER
	using UnityEditor.SceneManagement;
#else
	using UnityEditor.Experimental.SceneManagement;
#endif

	internal static class CSPrefabTools
	{
		public static PrefabInstanceStatus GetInstanceStatus(Object componentOrGameObject)
		{
			return PrefabUtility.GetPrefabInstanceStatus(componentOrGameObject);
		}
		
		public static bool IsProperlyConnectedInstance(PrefabInstanceStatus status)
		{
			return status == PrefabInstanceStatus.Connected;
		}
		
		public static bool IsInstance(PrefabInstanceStatus status)
		{
			return status != PrefabInstanceStatus.NotAPrefab;
		}

		public static bool IsMissingPrefabInstance(GameObject gameObject)
		{
			var status = PrefabUtility.GetPrefabInstanceStatus(gameObject);
			return status == PrefabInstanceStatus.MissingAsset ||
			       status == PrefabInstanceStatus.NotAPrefab && IsInstance(gameObject) && PrefabUtility.GetNearestPrefabInstanceRoot(gameObject) == null;
		}

		/*public static bool IsInstance(Object componentOrGameObject)
		{
			return PrefabUtility.IsPartOfVariantPrefab(componentOrGameObject) ? 
				PrefabUtility.IsPartOfNonAssetPrefabInstance(componentOrGameObject) :
				PrefabUtility.IsPartOfPrefabInstance(componentOrGameObject);
		}*/
		
		public static bool IsInstance(Object componentOrGameObject)
		{
			return PrefabUtility.IsPartOfNonAssetPrefabInstance(componentOrGameObject) ||
				PrefabUtility.IsPartOfPrefabInstance(componentOrGameObject);
		}

		/*public static Object GetAssetSource(Object componentOrGameObject)
		{
			return PrefabUtility.IsPartOfVariantPrefab(componentOrGameObject) ? 
				PrefabUtility.GetCorrespondingObjectFromSource(componentOrGameObject) : 
				PrefabUtility.GetCorrespondingObjectFromOriginalSource(componentOrGameObject);
		}*/
		
		public static Object GetAssetSource(Object componentOrGameObject)
		{
			return PrefabUtility.GetCorrespondingObjectFromSource(componentOrGameObject);
		}

		public static GameObject GetPrefabAssetRoot(string path)
		{
			return AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
		}

		public static GameObject OpenPrefabAndReturnRoot(string path, out bool prefabOpened)
		{
			var targetAsset = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
			var prefabType = PrefabUtility.GetPrefabAssetType(targetAsset);
			prefabOpened = false;

			GameObject result;

			if (prefabType == PrefabAssetType.Model)
			{
				result = targetAsset;
			}
			else
			{
				var prefabNeedsToBeOpened = true;

				var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				if (prefabStage != null)
				{
#if UNITY_2020_1_OR_NEWER
					if (prefabStage.assetPath == path)
#else
					if (prefabStage.prefabAssetPath == path)
#endif
					{
						prefabNeedsToBeOpened = false;
					}
				}

				if (prefabNeedsToBeOpened)
				{
					prefabOpened = AssetDatabase.OpenAsset(targetAsset);
					if (!prefabOpened)
					{
						Debug.LogError(Maintainer.ErrorForSupport("Couldn't open prefab at " + path));
						return null;
					}
				}

				prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				
				if (prefabStage == null)
				{
					Debug.LogError(Maintainer.ErrorForSupport("Couldn't get prefab stage for prefab at " + path));
					return null;
				}

				prefabOpened = true;
				result = prefabStage.prefabContentsRoot;
			}

			if (result == null)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Couldn't find prefab root at " + path));
				return null;
			}

			return result;
		}

		public static GameObject GetInstanceRoot(GameObject gameObject)
		{
			return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
		}

		public static bool IsInstanceRoot(GameObject gameObject)
		{
			return PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject);
		}

		public static bool IsWholeInstanceHasAnyOverrides(GameObject current)
		{
			return PrefabUtility.HasPrefabInstanceAnyOverrides(current, false);
		}

		public static void GetOverridenObjectsFromWholePrefabInstance(GameObject target, out int[] dirtyComponents)
		{
			var resultComponents = new HashSet<int>();

			var objectOverrides = PrefabUtility.GetObjectOverrides(target);
			foreach (var objectOverride in objectOverrides)
			{
				var component = objectOverride.instanceObject as Component;
				if (component != null)
				{
					resultComponents.Add(component.GetInstanceID());
				}
			}

			var addedComponents = PrefabUtility.GetAddedComponents(target);
			foreach (var addedComponent in addedComponents)
			{
				if (addedComponent != null)
				{
					resultComponents.Add(addedComponent.instanceComponent.GetInstanceID());
				}
			}
			
			var addedGameObjects = PrefabUtility.GetAddedGameObjects(target);
			foreach (var addedGameObject in addedGameObjects)
			{
				var components = addedGameObject.instanceGameObject.GetComponentsInChildren<Component>(true);
				foreach (var component in components)
				{
					if (component != null)
					{
						resultComponents.Add(component.GetInstanceID());
					}
				}
			}

			dirtyComponents = resultComponents.Count == 0 ? null : resultComponents.ToArray();
		}
	}
}
