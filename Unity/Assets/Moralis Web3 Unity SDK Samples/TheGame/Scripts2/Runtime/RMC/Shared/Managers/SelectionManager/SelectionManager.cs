
using UnityEngine.Events;

namespace RMC.Shared.Managers
{
	//  Namespace Properties ------------------------------
	public interface ISelectionManagerSelectable
	{
		bool IsSelected {set; get; }
	}

	public class SelectionUnityEvent : UnityEvent<ISelectionManagerSelectable>
	{
		
	}

	//  Class Attributes ----------------------------------


	/// <summary>
	/// Allows for 0 or 1 object to be selected at a time.
	/// </summary>
	public class SelectionManager 
	{
		//  Events ----------------------------------------
		public SelectionUnityEvent OnSelectionChanged = new SelectionUnityEvent();


		//  Properties ------------------------------------
		public ISelectionManagerSelectable Selection
		{
			set
			{
				if (_selection != null)
				{
					_selection.IsSelected = false;
				}

				_selection = value;
				
				if (_selection != null)
				{
					_selection.IsSelected = true;
				}
				
				OnSelectionChanged.Invoke(_selection);
			}
			get
			{
				
				return _selection;
			}
		}
		public static SelectionManager Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new SelectionManager();
				}
				return _Instance;
			}
		}

		//  Fields ----------------------------------------

		private ISelectionManagerSelectable _selection;
		private static SelectionManager _Instance;

		//  Methods ---------------------------------------
		public bool HasSelection()
		{
			return Selection != null;
		}
		
		public void ClearSelection()
		{
			Selection = null;
		}
		
		//  Event Handlers --------------------------------
	}
}
