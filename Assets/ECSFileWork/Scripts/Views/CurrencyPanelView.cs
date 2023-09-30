using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nashet.ECSFileWork.Views
{
	public delegate void ButtonClickedDelegate();

	public class CurrencyPanelView : MonoBehaviour
	{
		[SerializeField] private TMPro.TextMeshProUGUI currencyText;
		[SerializeField] private TMPro.TextMeshProUGUI currencyAmount;

		public event ButtonClickedDelegate OnCurrenceIncreaseClicked;
		public event ButtonClickedDelegate OnCurrnecySetZeroClicked;
		public void OnCurrenceIncreaseClickedHandler()
		{
			OnCurrenceIncreaseClicked?.Invoke();
		}

		public void OnCurrnecySetZeroClickedHandler()
		{
			OnCurrnecySetZeroClicked?.Invoke();
		}
	}
}