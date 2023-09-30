using System.Runtime.InteropServices;
using Unity.Entities;

namespace Nashet.ECSFileWork.ECS
{
	public struct WalletComponent : IComponentData
	{
		public int currencyId;
		public int amount;
	}
}
