#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Extension
{
	public interface IMaintainerExtension
	{
		// TODO: with C# 8
		// bool External { internal set; get; }	
		bool External { set; get; }	
		bool Enabled { set; get; }	
		string Id { get; }
	}
}