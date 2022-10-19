#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Extension
{
	/// <summary>
	/// Use for all Maintainer extensions in order to automatically implement IMaintainerExtension
	/// </summary>
	public abstract class MaintainerExtension : IMaintainerExtension
	{
		protected abstract bool Enabled { get; set; }
		private string Id { get; set; }

		bool IMaintainerExtension.External { get; set; }
		
		bool IMaintainerExtension.Enabled
		{
			get { return Enabled; }
			set { Enabled = value; }
		}
		
		string IMaintainerExtension.Id
		{
			get
			{
				if (Id == null)
					Id = GetId(this);
				return Id;
			}
		}
		
		internal static string GetId(IMaintainerExtension instance)
		{
			return instance.GetType().Name;
		}
		
		internal static string GetId<T>() where T : IMaintainerExtension
		{
			return typeof(T).Name;
		}
	}
}