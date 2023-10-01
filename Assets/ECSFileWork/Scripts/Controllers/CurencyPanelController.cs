using Nashet.ECSFileWork.ECS;
using Nashet.ECSFileWork.Views;
using System;
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
		[SerializeField] private DotsEventsController dotsEventsController;

		public event CurrencyAmountChangedDelegate OnCurrencyAmountChanged; //todo rise it. Or not?

		private EntityManager entityManager;
		private int currentValue = 0;

		// Start is called before the first frame update
		void Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			RegisterUsedCurrencyId();

			curencyPanelView.SetCurrencyText($"{currencyName}:");

			curencyPanelView.OnCurrencyIncreaseClicked += CurrencyIncreaseClickedHandler;
			curencyPanelView.OnCurrnecySetZeroClicked += CurrencySetZeroClickedHandler;
			dotsEventsController.OnDataLoaded += DataLoadedHandler;
		} //todo add OnDestroy

		private void RegisterUsedCurrencyId()
		{
			Entity entity = entityManager.CreateEntity(typeof(WalletComponent));
			var myComponent = entityManager.GetComponentData<WalletComponent>(entity);
			myComponent.currencyId = currencyId;
			entityManager.SetComponentData(entity, myComponent);
		}

		private void DataLoadedHandler()
		{
			var foundEntity = FoundWalletEntityById();
			
			if (foundEntity != Entity.Null)
			{
				var myComponent = entityManager.GetComponentData<WalletComponent>(foundEntity);

				curencyPanelView.SetCurrencyAmount(myComponent.amount.ToString());
				currentValue = myComponent.amount;
			}
		}

		private void CurrencySetZeroClickedHandler()
		{
			SetValueInModel(0);
		}

		private void CurrencyIncreaseClickedHandler()
		{
			SetValueInModel(currentValue + 1);
		}

		private void SetValueInModel(int value)
		{
			var foundEntity = FoundWalletEntityById();

			// Modify the desired component value
			if (foundEntity != Entity.Null)
			{
				var myComponent = entityManager.GetComponentData<WalletComponent>(foundEntity);
				myComponent.amount = value;
				entityManager.SetComponentData(foundEntity, myComponent);
				DotsEventsController.Instance.RiseOnDataLoaded();
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