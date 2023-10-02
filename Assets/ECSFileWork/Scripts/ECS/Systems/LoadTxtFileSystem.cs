using Nashet.ECSFileWork.Controllers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nashet.ECSFileWork.ECS
{
	public partial class LoadTxtFileSystem : SystemBase
	{
		private EntityQuery entityQuery;
		private EntityManager entityManager;
		private Task<WalletComponentListWrapper> loadingTask;
		private string filePath = Application.persistentDataPath + "/data.txt";// "/<.>&3`~~.txt"; // 

		private bool isLoadingInProcess => loadingTask != null && !loadingTask.IsCompleted;
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
				if (entities.Length == 0)
					return;

				if (loadingTask == null)
					loadingTask = LoadData(filePath);

				if (isLoadingInProcess)
					return;

				if (loadingTask.IsFaulted)
				{
					var exception = loadingTask.Exception;
					RemoveLoadingFlag(entities);
					if (exception != null)
						throw exception;
					return;
				}

				var loadedData = loadingTask.Result.WalletComponents.ToDictionary(x => x.currencyId);

				// Iterate over the entities
				for (int i = 0; i < entities.Length; i++)
				{
					Entity entity = entities[i];
					var wallet = entityManager.GetComponentData<WalletComponent>(entity);
					wallet.amount = loadedData[wallet.currencyId].amount; //todo it is expected to rise exception if there is no that key

					// Write the updated value back to the component
					entityManager.SetComponentData(entity, wallet);

					EntityManager.RemoveComponent<LoadFlagComponent>(entity);
				}
				DotsEventsController.Instance.RiseOnDataLoaded();

				loadingTask = null;
			}
		}

		private void RemoveLoadingFlag(NativeArray<Entity> entities)
		{
			loadingTask = null;
			for (int i = 0; i < entities.Length; i++)
			{
				Entity entity = entities[i];
				EntityManager.RemoveComponent<LoadFlagComponent>(entity);
			}
		}

		public async Task<WalletComponentListWrapper> LoadData(string filePath)
		{
			string jsonData;
			WalletComponentListWrapper walletComponentList;
			try
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					jsonData = await reader.ReadToEndAsync();
				}

				// Perform deserialization
				walletComponentList = JsonUtility.FromJson<WalletComponentListWrapper>(jsonData);
			}
			// exceptions from async code might be "swallowed" so its better to remind:
			catch (IOException ex)
			{
				// Handle IOException here
				Debug.LogError("IO error occurred while loading data from file: " + ex.Message);
				throw;
			}
			catch (Exception ex)
			{
				// Handle any other exceptions here
				Debug.LogError("Error occurred while loading data to file: " + ex.Message);
				throw;
			}

			Debug.Log($"Finished loading data from {filePath}");
			return walletComponentList;
		}
	}
}
