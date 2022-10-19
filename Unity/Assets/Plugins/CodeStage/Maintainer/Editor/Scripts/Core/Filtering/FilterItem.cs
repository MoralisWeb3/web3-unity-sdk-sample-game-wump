#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core
{
	using System;

	[Serializable]
	public enum FilterKind
	{
		Path,
		Directory,
		FileName,
		Extension,
		Type,

		NotSet = 1000,
	}

	[Serializable]
	public class FilterItem
	{
		public bool enabled = true;
		public bool ignoreCase;
		public FilterKind kind;
		public string value;
		public bool exactMatch;

		[NonSerialized]
		internal bool isEditingValue;

		public static FilterItem Create(string value, FilterKind kind, bool ignoreCase = false, bool exactMatch = false)
		{
			return Create(true, value, kind, ignoreCase, exactMatch);
		}
		
		public static FilterItem Create(bool enabled, string value, FilterKind kind, bool ignoreCase = false, bool exactMatch = false)
		{
			var newFilter = new FilterItem
			{
				enabled = enabled,
				ignoreCase = ignoreCase,
				kind = kind,
				value = value,
				exactMatch = exactMatch
			};

			return newFilter;
		}
	}
}