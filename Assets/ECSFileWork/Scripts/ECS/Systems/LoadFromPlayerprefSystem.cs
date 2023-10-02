using Nashet.ECSFileWork.Controllers;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nashet.ECSFileWork.ECS
{
	public partial class LoadFromPlayerprefSystem : SystemBase
	{
		private EntityQuery entityQuery;
		private EntityManager entityManager;

		protected override void OnCreate()
		{
			// Define the query to retrieve entities with the component to remove
			entityQuery = GetEntityQuery(typeof(LoadFlagComponent));
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		protected override void OnUpdate()
		{
			// Get the array of entities that match the query
			using (var entities = entityQuery.ToEntityArray(Allocator.TempJob))
			{
				var loadedCounter = 0;
				// Iterate over the entities
				for (int i = 0; i < entities.Length; i++) //note, we only loading data for registered currencies.
				{
					Entity entity = entities[i];
					var wallet = entityManager.GetComponentData<WalletComponent>(entity);

					var key = "Currency" + wallet.currencyId;
					try
					{
						wallet.amount = PlayerPrefs.GetInt(key); //todo that might break data locality

						entityManager.SetComponentData(entity, wallet);
					}
					catch (Exception e)
					{
						Debug.LogError($"Error loading PlayerPrefs key {key}: {e.Message}");
						RemoveLoadingFlag(entities);
						throw;
					}

					loadedCounter++;

					EntityManager.RemoveComponent<LoadFlagComponent>(entity);
				}

				if (loadedCounter > 0)
				{
					Debug.Log($"Finished loading data from playerPrefs");

					DotsEventsController.Instance.RiseOnDataLoaded();
				}
			}
		}

		private void RemoveLoadingFlag(NativeArray<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				Entity entity = entities[i];
				EntityManager.RemoveComponent<LoadFlagComponent>(entity);
			}
		}
	}
}