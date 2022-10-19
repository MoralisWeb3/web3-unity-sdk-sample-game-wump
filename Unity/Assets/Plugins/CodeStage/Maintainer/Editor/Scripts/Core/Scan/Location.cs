#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	using System;
	using Core;
	using UnityEditor;
	using UnityEngine;
	
	// TODO: try switching to struct and compare performance

	internal class NarrowLocation : GenericLocation
	{
		public void PropertyOverride(SerializedProperty property)
		{
			PropertyBegin(property);
		}
		
		public void PropertyOverride(string propertyPathOverride)
		{
			Property = null;
			propertyPath = propertyPathOverride;
		}
		
		public void PropertyOverride(SerializedProperty property, string propertyPathOverride)
		{
			Property = property;
			propertyPath = propertyPathOverride;
		}
		
		public void ComponentOverride(Type typeOverride, string nameOverride, int index)
		{
			Component = null;
			ComponentType = typeOverride;
			ComponentName = nameOverride;
			ComponentIndex = index;
		}
		
		public void ComponentOverride(string nameOverride)
		{
			Component = null;
			ComponentType = null;
			ComponentName = nameOverride;
			ComponentIndex = -1;
		}
	}
	
	internal class GenericLocation : PropertyLocation
	{
		
	}

	internal class PropertyLocation : ComponentLocation
	{
		protected string propertyPath;
		
		public SerializedProperty Property { get; protected set; }
		public string PropertyPath 
		{
			get
			{
				if (string.IsNullOrEmpty(propertyPath) && Property != null)
					propertyPath = Property.propertyPath;

				return propertyPath;
			}
		}
		
		protected PropertyLocation() { } // to avoid explicit construction

		internal void PropertyBegin(SerializedProperty property)
		{
			Property = property;
			propertyPath = null;
		}
		
		internal void PropertyEnd()
		{
			Property = null;
			propertyPath = null;
		}
		
		internal void FillFrom(PropertyLocation source)
		{
			Property = source.Property;
			propertyPath = source.PropertyPath;
			FillFrom(source as ComponentLocation);
		}
	}

	internal class ComponentLocation : GameObjectLocation
	{
		public Component Component { get; protected set; }
		public Type ComponentType { get; protected set; }
		public string ComponentName { get; protected set; }
		public int ComponentIndex { get; protected set; }
		
		protected ComponentLocation() { } // to avoid explicit construction
		
		internal void ComponentBegin(Component component, int index)
		{
			Component = component;
			if (component != null)
			{
				ComponentType = component.GetType();
				ComponentName = ComponentType.Name;
			}
			ComponentIndex = index;
		}
		
		internal void ComponentEnd()
		{
			Component = null;
			ComponentType = null;
			ComponentName = null;
			ComponentIndex = -1;
		}
		
		internal void FillFrom(ComponentLocation source)
		{
			Component = source.Component;
			ComponentType = source.ComponentType;
			ComponentName = source.ComponentName;
			ComponentIndex = source.ComponentIndex;
			FillFrom(source as GameObjectLocation);
		}
	}

	internal class GameObjectLocation : AssetLocation
	{
		protected GameObjectLocation() { } // to avoid explicit construction
		
		public GameObject GameObject { get; private set; }
		
		internal void GameObjectBegin(GameObject target)
		{
			GameObject = target;
		}
		
		internal void GameObjectEnd()
		{
			GameObject = null;
		}
		
		internal void FillFrom(GameObjectLocation source)
		{
			GameObject = source.GameObject;
			FillFrom(source as AssetLocation);
		}
	}

	internal class AssetLocation : Location
	{
		public AssetInfo Asset { get; private set; }

		protected AssetLocation() { } // to avoid explicit construction
		
		internal void AssetBegin(LocationGroup group, AssetInfo asset)
		{
			Group = group;
			Asset = asset;
		}

		internal void AssetEnd()
		{
			Group = LocationGroup.Unknown;
			Asset = null;
		}
		
		internal void FillFrom(AssetLocation source)
		{
			Asset = source.Asset;
			FillFrom(source as Location);
		}
	}

	internal class Location
	{
		public LocationGroup Group { get; protected set; }
		
		protected Location() { } // to avoid explicit construction

		internal void FillFrom(Location source)
		{
			Group = source.Group;
		}
		
		public NarrowLocation Narrow()
		{
			var newCopy = new NarrowLocation();
			newCopy.FillFrom(this as GenericLocation);
			return newCopy;
		}
	}
}