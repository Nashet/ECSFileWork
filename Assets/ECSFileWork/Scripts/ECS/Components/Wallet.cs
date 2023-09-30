using System.Runtime.InteropServices;
using Unity.Entities;

namespace Nashet.ECSFileWork.ECS
{
	public struct Wallet : IComponentData
	{
		public int currencyId;
		public int amount;
	}
}
