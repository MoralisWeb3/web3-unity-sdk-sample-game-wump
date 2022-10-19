#region copyright
// ---------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// ---------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Dependencies
{
	using System.Collections.Generic;
	using System.Linq;
	using Extension;
	using Tools;
	using UnityEditor;

	/// <summary>
	/// Does looks for assets dependencies used at the Maintainer's Assets Map.
	/// </summary>
	public static class AssetDependenciesSearcher
	{
		private static ExtensibleModule<IDependenciesParser> parsersHolder;

#if UNITY_2019_2_OR_NEWER
		static AssetDependenciesSearcher()
		{
			parsersHolder = new ExtensibleModule<IDependenciesParser>();
		}

		[System.Obsolete("This method is obsolete now. Parser will register itself automatically.", true)]
		public static void AddExternalDependenciesParser(IDependenciesParser parser) { }

		[System.Obsolete("This method is obsolete now. Parser will register itself automatically.", true)]
		public static void AddExternalDependenciesParsers(IList<IDependenciesParser> parsers) { }
#else
		public static void AddExternalDependenciesParser(IDependenciesParser parser)
		{
			if (parsersHolder == null)
				parsersHolder = new ExtensibleModule<IDependenciesParser>();

			parsersHolder.AddExternal(parser);
		}
		
		/// <summary>
		/// Register any custom dependencies parsers here. Allows parsing unknown types dependencies.
		/// </summary>
		/// Passed parsers will be used in the Assets Dependencies Map and will affect all Maintainer references-related functionality.<br/>
		/// Call this before running Maintainer first time (before Assets Map is created).<br/>
		/// For example, call it from the [InitializeOnLoad] class' static constructor.
		/// <param name="parsers">List of new parsers.</param>
		public static void AddExternalDependenciesParsers(IList<IDependenciesParser> parsers)
		{
			if (parsersHolder == null)
				parsersHolder = new ExtensibleModule<IDependenciesParser>();

			parsersHolder.AddExternal(parsers);
		}
		
		// gets called from internal parsers
		internal static void AddInternalDependencyParser(IDependenciesParser parser)
		{
			if (parsersHolder == null)
				parsersHolder = new ExtensibleModule<IDependenciesParser>();

			parsersHolder.AddInternal(parser);
		}
#endif

		internal static string[] FindDependencies(AssetInfo asset)
		{
			var dependenciesGUIDs = new HashSet<string>();
			
			/* pre-regular dependenciesGUIDs additions */
			FillDependencies(parsersHolder.extensions, ref dependenciesGUIDs, asset);

			if (asset.Kind != AssetKind.Settings)
			{
				/* regular dependenciesGUIDs additions */
				var dependencies = AssetDatabase.GetDependencies(asset.Path, false);
				var guids = CSAssetTools.GetAssetsGUIDs(dependencies);
				if (guids != null && guids.Length > 0)
				{
					dependenciesGUIDs.UnionWith(guids);
				}
			}
			
			// kept for debugging purposes
			/*if (Path.Contains("1.unity"))
			{
				Debug.Log("1.unity non-recursive dependenciesGUIDs:");
				foreach (var reference in references)
				{
					Debug.Log(reference);
				}
			}*/

			return dependenciesGUIDs.ToArray();
		}

		private static void FillDependencies(List<IDependenciesParser> dependenciesParsers, ref HashSet<string> dependenciesGUIDs, AssetInfo asset)
		{
			if (dependenciesParsers == null)
				return;
			
			foreach (var parser in dependenciesParsers)
			{
				if (parser.Type != null && parser.Type != asset.Type)
					continue;

				var foundDependencies = parser.GetDependenciesGUIDs(asset);
				if (foundDependencies == null || foundDependencies.Count == 0)
					continue;
				
				dependenciesGUIDs.UnionWith(foundDependencies);
			}
		}
	}
}