#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using System.Text;
	using Issues;
	using UnityEditor;
	using UnityEngine;

	public static class CSSerializedPropertyTools
	{
		public static bool IsPropertyHasMissingReference(SerializedProperty currentProperty)
		{
			if (currentProperty.propertyType != SerializedPropertyType.ObjectReference || 
				currentProperty.objectReferenceValue != null) 
				return false;
			
			if (currentProperty.objectReferenceInstanceIDValue != 0)
				return true;

			var fileId = currentProperty.FindPropertyRelative("m_FileID");
			if (fileId != null)
			{
				if (fileId.intValue != 0) 
					return true;
			}
			else
			{
				Debug.LogError(Maintainer.ErrorForSupport("Property seems to be missing reference but m_FileID could not be found!",
					IssuesFinder.ModuleName));
			}

			return false;
		}
		
		// TODO: compare duplicate search performance at 1.2.x and 1.3.0 in Unity 2022.1
#if UNITY_2022_1_OR_NEWER
		public static uint GetPropertyHash(SerializedProperty sp)
		{
			return sp.contentHash;
#else
		public static int GetPropertyHash(SerializedProperty sp)
		{
			var stringHash = new StringBuilder();
			stringHash.Append(sp.type);

			if (sp.isArray)
			{
				stringHash.Append(sp.arraySize);
			}
			else
			{
				switch (sp.propertyType)
				{
					case SerializedPropertyType.AnimationCurve:
						if (sp.animationCurveValue != null)
						{
							stringHash.Append(sp.animationCurveValue.length);
							if (sp.animationCurveValue.keys != null)
							{
								foreach (var key in sp.animationCurveValue.keys)
								{
									stringHash.Append(key.value)
											  .Append(key.time)
											  .Append(key.outTangent)
											  .Append(key.inTangent);
								}
							}
						}
						break;
					case SerializedPropertyType.ArraySize:
						stringHash.Append(sp.intValue);
						break;
					case SerializedPropertyType.Boolean:
						stringHash.Append(sp.boolValue);
						break;
					case SerializedPropertyType.Bounds:
						stringHash.Append(sp.boundsValue.GetHashCode());
						break;
					case SerializedPropertyType.Character:
						stringHash.Append(sp.intValue);
						break;
					case SerializedPropertyType.Generic: // looks like arrays which we already walk through
						break;
					case SerializedPropertyType.Gradient: // unsupported
						break;
					case SerializedPropertyType.ObjectReference:
						if (sp.objectReferenceValue != null)
							stringHash.Append(sp.objectReferenceValue.GetInstanceID());
						break;
					case SerializedPropertyType.Color:
						stringHash.Append(sp.colorValue.GetHashCode());
						break;
					case SerializedPropertyType.Enum:
						stringHash.Append(sp.enumValueIndex);
						break;
					case SerializedPropertyType.Float:
						stringHash.Append(sp.floatValue);
						break;
					case SerializedPropertyType.Integer:
						stringHash.Append(sp.intValue);
						break;
					case SerializedPropertyType.LayerMask:
						stringHash.Append(sp.intValue);
						break;
					case SerializedPropertyType.Quaternion:
						stringHash.Append(sp.quaternionValue.GetHashCode());
						break;
					case SerializedPropertyType.Rect:
						stringHash.Append(sp.rectValue.GetHashCode());
						break;
					case SerializedPropertyType.String:
						stringHash.Append(sp.stringValue);
						break;
					case SerializedPropertyType.Vector2:
						stringHash.Append(sp.vector2Value.GetHashCode());
						break;
					case SerializedPropertyType.Vector3:
						stringHash.Append(sp.vector3Value.GetHashCode());
						break;
					case SerializedPropertyType.Vector4:
						stringHash.Append(sp.vector4Value.GetHashCode());
						break;
					case SerializedPropertyType.ExposedReference:
						if (sp.exposedReferenceValue != null)
							stringHash.Append(sp.exposedReferenceValue.GetInstanceID());
						break;
					case SerializedPropertyType.Vector2Int:
						stringHash.Append(sp.vector2IntValue.GetHashCode());
						break;
					case SerializedPropertyType.Vector3Int:
						stringHash.Append(sp.vector3IntValue.GetHashCode());
						break;
					case SerializedPropertyType.RectInt:
						stringHash.Append(sp.rectIntValue.position.GetHashCode()).Append(sp.rectIntValue.size.GetHashCode());
						break;
					case SerializedPropertyType.BoundsInt:
						stringHash.Append(sp.boundsIntValue.GetHashCode());
						break;
					case SerializedPropertyType.FixedBufferSize:
						stringHash.Append(sp.fixedBufferSize);
						break;
#if UNITY_2019_3_OR_NEWER
					case SerializedPropertyType.ManagedReference:
						stringHash.Append(sp.managedReferenceFullTypename);
						break;
#endif
					default:
						Debug.LogWarning(Maintainer.ErrorForSupport("Unknown SerializedPropertyType: " + sp.propertyType));
						break;
				}
			}

			return stringHash.ToString().GetHashCode();
#endif
		}
	}
}