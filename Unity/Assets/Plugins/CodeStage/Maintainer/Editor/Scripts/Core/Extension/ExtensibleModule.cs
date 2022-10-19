#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Extension
{
	using System;
	using System.Collections.Generic;
#if UNITY_2019_2_OR_NEWER
	using UnityEditor;
	using UnityEngine;
#endif

	internal class ExtensibleModule<T> where T : IMaintainerExtension
	{
		public readonly List<T> extensions = new List<T>();
		
#if !UNITY_2019_2_OR_NEWER
		private readonly IComparer<T> comparer;
#endif
		
		public ExtensibleModule(IComparer<T> comparer = null)
		{
#if UNITY_2019_2_OR_NEWER
			var targetType = typeof(T);
			var foundTypes = TypeCache.GetTypesDerivedFrom<T>();
			foreach (var foundType in foundTypes)
			{
				if (foundType.IsInterface || foundType.IsAbstract)
					continue;
				
				var extensionInstance = (T)Activator.CreateInstance(foundType);
				if (targetType.Namespace == foundType.Namespace)
				{
					// Debug.Log("Internal " + targetType.Name + " extension registered: " + foundType);
					AddInternal(extensionInstance);
				}
				else
				{
					// Debug.Log("External " + targetType.Name + " extension registered: " + foundType);
					AddExternal(extensionInstance);
				}
			}
			
			if (comparer != null)
				extensions.Sort(comparer);
#else
			this.comparer = comparer;
#endif
		}

		internal void AddInternal(T extension)
		{
			if (extension == null)
				throw new ArgumentNullException("extension");

			Add(extension);
		}

		internal void AddInternal(IList<T> newExtensions)
		{
			if (newExtensions == null || newExtensions.Count == 0)
				throw new ArgumentNullException("newExtensions");

			AddCollection(newExtensions);
		}
		
		public void AddExternal(T extension)
		{
			if (extension == null)
				throw new ArgumentNullException("extension");
			
			Add(extension, true);
		}

		public void AddExternal(IList<T> newExtensions)
		{
			if (newExtensions == null || newExtensions.Count == 0)
				throw new ArgumentNullException("newExtensions");
			
			AddCollection(newExtensions, true); 
		}
		
		private void Add(T extension, bool external = false)
		{
			if (!extensions.Contains(extension))
			{
				extension.External = external;
				extensions.Add(extension);
#if !UNITY_2019_2_OR_NEWER
				if (comparer != null)
					extensions.Sort(comparer);
#endif
			}
		}
		
		private void AddCollection(IList<T> list, bool external = false)
		{
			for (var i = 0; i < list.Count; i++)
			{
				var extension = list[i];
				Add(extension, external);
			}
		}
		
		internal class DefaultComparer : IComparer<T>
		{
			public int Compare(T x, T y)
			{
				if (ReferenceEquals(x, y))
					return 0;

				if (ReferenceEquals(x, null))
					return 0;

				if (ReferenceEquals(y, null))
					return 0;

				return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
			}
		}
	}
}