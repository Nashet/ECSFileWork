using System;
using Unity.Entities;

namespace Nashet.ECSFileWork.ECS
{
	[Serializable]
	public struct WalletComponent : IComponentData
	{
		public int currencyId;
		public int amount;
	}
}
