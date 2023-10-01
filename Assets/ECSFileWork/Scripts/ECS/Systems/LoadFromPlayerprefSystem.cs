using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Nashet.ECSFileWork.Controllers;

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
			var entities = entityQuery.ToEntityArray(Allocator.TempJob);
			var loadedCounter = 0;
			// Iterate over the entities
			for (int i = 0; i < entities.Length; i++) //note, we only loading data for registered currencies.
			{
				Entity entity = entities[i];
				var wallet = entityManager.GetComponentData<WalletComponent>(entity);

				var key = "Currency" + wallet.currencyId;//todo improve key
				try
				{
					wallet.amount = PlayerPrefs.GetInt(key); //todo that break data locality
					// Write the updated value back to the component
					entityManager.SetComponentData(entity, wallet);
				}
				catch (Exception e)
				{
					Debug.LogError($"Error loading PlayerPrefs key {key}: {e.Message}");
					throw;
				}

				loadedCounter++;

				EntityManager.RemoveComponent<LoadFlagComponent>(entity);
			}

			if (loadedCounter > 0)
			{
				DotsEventsController.Instance.RiseOnDataLoaded();
			}

			entities.Dispose();
		}
	}
}