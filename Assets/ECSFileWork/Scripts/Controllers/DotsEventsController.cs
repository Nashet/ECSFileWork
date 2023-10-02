namespace Nashet.ECSFileWork.Controllers
{
	public delegate void DataLoadedDelegate();
	public class DotsEventsController : MonoSingleton<DotsEventsController>
	{
		public event DataLoadedDelegate OnDataLoaded;

		internal void RiseOnDataLoaded()
		{
			OnDataLoaded?.Invoke();
		}
	}
}