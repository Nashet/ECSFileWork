using Nashet.ECSFileWork.Views;
using UnityEngine;

namespace Nashet.ECSFileWork.Controllers
{
	public class ExceptionHandlerController : MonoBehaviour
	{
		[SerializeField] ExceptionHandlerView exceptionHandlerView;
		private void Awake()
		{
			Application.logMessageReceived += HandleLogMessage;
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= HandleLogMessage;
		}

		private void HandleLogMessage(string logString, string stackTrace, LogType logType)
		{
			if (logType == LogType.Exception)
			{
				exceptionHandlerView.SetText(logString);
			}
		}
	}
}