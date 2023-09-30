using Nashet.ECSFileWork.ECS;
using Nashet.ECSFileWork.Views;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nashet.ECSFileWork.Controllers
{
	public delegate void CurrencyAmountChangedDelegate(int amount, int currencyId);

	public class CurencyPanelController : MonoBehaviour
	{
		[SerializeField] private CurrencyPanelView curencyPanelView; //todo it might be good to use interface here
		[SerializeField] private string currencyName;
		[SerializeField] private int currencyId;

		public event CurrencyAmountChangedDelegate OnCurrencyAmountChanged; //todo rise it. Or not?

		private EntityManager entityManager;

		// Start is called before the first frame update
		void Start()
		{
			var defaultvalue = 0;
			curencyPanelView.SetCurrencyText($"{currencyName}:");
			curencyPanelView.SetCurrencyAmount(defaultvalue.ToString());

			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

			Entity entity = entityManager.CreateEntity(typeof(WalletComponent));

			var myComponent = entityManager.GetComponentData<WalletComponent>(entity);

			myComponent.currencyId = currencyId;
			myComponent.amount = defaultvalue;

			entityManager.SetComponentData(entity, myComponent);

			curencyPanelView.OnCurrencyIncreaseClicked += CurrencyIncreaseClickedHandler;
			curencyPanelView.OnCurrnecySetZeroClicked += CurrencySetZeroClickedHandler;
		}

		private void CurrencySetZeroClickedHandler()
		{
			var foundEntity = FoundWalletEntityById();

			// Modify the desired component value
			if (foundEntity != Entity.Null)
			{
				var myComponent = entityManager.GetComponentData<WalletComponent>(foundEntity);
				myComponent.amount = 0;
				entityManager.SetComponentData(foundEntity, myComponent);
			}
		}

		private void CurrencyIncreaseClickedHandler()
		{
			var foundEntity = FoundWalletEntityById();

			// Modify the desired component value
			if (foundEntity != Entity.Null)
			{
				var myComponent = entityManager.GetComponentData<WalletComponent>(foundEntity);
				myComponent.amount++;
				entityManager.SetComponentData(foundEntity, myComponent);
			}
		}

		private Entity FoundWalletEntityById()
		{
			// Query the entity with the desired field value
			EntityQuery query = entityManager.CreateEntityQuery(
				ComponentType.ReadOnly<WalletComponent>()
			);

			NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
			Entity foundEntity = Entity.Null;

			for (int i = 0; i < entities.Length; i++)
			{
				Entity entity = entities[i];
				var myComponent = entityManager.GetComponentData<WalletComponent>(entity); //todo fix that

				if (myComponent.currencyId == currencyId)
				{
					foundEntity = entity;
					break;
				}
			}

			entities.Dispose();
			return foundEntity;
		}
	}
}