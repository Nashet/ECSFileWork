using TMPro;
using UnityEngine;

namespace Nashet.ECSFileWork.Views
{
	public class ExceptionHandlerView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;

		public void SetText(string newtext)
		{
			text.text = newtext;
		}
	}
}