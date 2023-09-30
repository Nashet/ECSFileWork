using UnityEngine;

namespace Nashet.ECSFileWork.Views
{
	public delegate void DropdownChangedDelegate(int value);
	public class SaveLoadPanelView : MonoBehaviour
	{
		public event ButtonClickedDelegate OnSaveClicked;
		public event ButtonClickedDelegate OnLoadClicked;
		public event DropdownChangedDelegate OnDropdownValueChanged;
		public void OnSaveClickedHandler()
		{
			OnSaveClicked?.Invoke();
		}

		public void OnLoadClickedHandler()
		{
			OnLoadClicked?.Invoke();
		}

		public void OnDropdownValueChangedHandler(int value)
		{
			OnDropdownValueChanged?.Invoke(value);
		}
	}
}