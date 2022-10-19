#region copyright
// ---------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// ---------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Dependencies
{
	using System;
	using System.Collections.Generic;
	using Extension;

	/// <summary>
	/// Interface used for all Dependencies Parsers.
	/// </summary>
	public interface IDependenciesParser : IMaintainerExtension
	{
		/// <summary>
		/// Parser target asset type.
		/// Return null to match all assets types.
		/// </summary>
		Type Type { get; }
		
		/// <summary>
		/// Called by Maintainer in order to get passed asset dependencies.
		/// </summary>
		/// <param name="asset">Asset information.</param>
		/// <returns>AssetDatabase GUIDs of all assets used in the target asset at the specified path.</returns>
		IList<string> GetDependenciesGUIDs(AssetInfo asset);
	}
}