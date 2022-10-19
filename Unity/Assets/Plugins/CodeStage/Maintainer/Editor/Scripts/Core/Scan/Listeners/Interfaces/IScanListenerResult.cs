#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Scan
{
	using System.Collections.Generic;

	public interface IScanListenerResult
	{
		
	}
	
	public interface IScanListenerResults{}
	
	public interface IScanListenerResults<T> : IScanListenerResults where T : IScanListenerResult
	{
		bool IsEmpty { get; }
		void Add(T result);
		void Add(IList<T> newResults);
		IList<T> Get();
	}
}