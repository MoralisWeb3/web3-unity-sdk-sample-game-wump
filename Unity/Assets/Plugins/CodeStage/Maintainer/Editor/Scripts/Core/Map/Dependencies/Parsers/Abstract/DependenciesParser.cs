#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Dependencies
{
	using System;
	using System.Collections.Generic;
	using Extension;

	/// <summary>
	/// Base class for all %Dependencies Parsers. Use to add your own %Dependencies Parsers extensions.
	/// </summary>
	public abstract class DependenciesParser : MaintainerExtension, IDependenciesParser
	{
		protected override bool Enabled
		{
			get { return true; }
			set { /* can't be disabled */ }
		}

		public abstract Type Type { get; }
		public abstract IList<string> GetDependenciesGUIDs(AssetInfo asset);
	}
}