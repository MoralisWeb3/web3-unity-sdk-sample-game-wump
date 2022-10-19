#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	using System.Collections.Generic;

	internal class ScanListenerResults<T> : IScanListenerResults<T> where T : IScanListenerResult
	{
		protected List<T> results;

		public bool IsEmpty
		{
			get
			{
				return results == null || results.Count == 0;
			}
		}
		
		public void Add(T newResult)
		{
			if (newResult == null)
				return;
			
			if (results == null)
				results = new List<T>();
			
			results.Add(newResult);
		}
		
		public void Add(IList<T> newResults)
		{
			if (newResults == null || newResults.Count == 0)
				return;
			
			if (results == null)
				results = new List<T>(newResults);
			else
				results.AddRange(newResults);
		}

		public IList<T> Get()
		{
			return results;
		}
	}
}