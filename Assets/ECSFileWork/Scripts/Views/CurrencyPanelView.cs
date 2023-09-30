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

		public event ButtonClickedDelegate OnCurrencyIncreaseClicked;
		public event ButtonClickedDelegate OnCurrnecySetZeroClicked;

		public void SetCurrencyText(string text)
		{
			currencyText.text = text;
		}

		public void SetCurrencyAmount(string amount)
		{
			currencyAmount.text = amount;
		}

		public void OnCurrenceIncreaseClickedHandler()
		{
			OnCurrencyIncreaseClicked?.Invoke();
		}

		public void OnCurrnecySetZeroClickedHandler()
		{
			OnCurrnecySetZeroClicked?.Invoke();
		}
	}
}