using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

namespace Nashet.ECSFileWork.ECS
{
	public partial class SaveToPlayerPrefsSystem : SystemBase
	{
		private EntityQuery entityQuery;
		private EntityManager entityManager;

		protected override void OnCreate()
		{
			// Define the query to retrieve entities with the component to remove
			entityQuery = GetEntityQuery(typeof(SaveFlagComponent));
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		protected override void OnUpdate()
		{
			// Get the array of entities that match the query
			var entities = entityQuery.ToEntityArray(Allocator.TempJob);
			var walletComponents = new List<WalletComponent>();
			// Iterate over the entities
			for (int i = 0; i < entities.Length; i++)
			{
				Entity entity = entities[i];
				var myComponent = entityManager.GetComponentData<WalletComponent>(entity);

				walletComponents.Add(myComponent);  //TODO Research for a way to get all components at once

				EntityManager.RemoveComponent<SaveFlagComponent>(entity);
			}
			SaveData(walletComponents);
			entities.Dispose();
		}

		public void SaveData(List<WalletComponent> walletComponents)
		{
			foreach (var wallet in walletComponents)
			{
				var key = "Currency" + wallet.currencyId;
				try
				{
					PlayerPrefs.SetInt(key, wallet.amount);
				}
				catch (Exception e)
				{
					Debug.LogError($"Error setting PlayerPrefs key {key}: {e.Message}");
				}
			}
			try
			{
				PlayerPrefs.Save();
			}
			catch (Exception e)
			{
				Debug.LogError($"Error setting PlayerPrefs key: {e.Message}");
			}

			if (walletComponents.Count != 0)
			{
				Debug.Log($"Finished saving data in PlayerPrefs");
			}
		}
	}
}