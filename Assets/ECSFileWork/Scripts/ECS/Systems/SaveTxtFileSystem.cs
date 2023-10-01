using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nashet.ECSFileWork.ECS
{
	public partial class SaveTxtFileSystem : SystemBase
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

				walletComponents.Add(myComponent);

				EntityManager.RemoveComponent<SaveFlagComponent>(entity);
			}
			if (walletComponents.Count > 0)
			{
				SaveData(walletComponents);
			}
			entities.Dispose();
		}

		public async Task SaveData(List<WalletComponent> walletComponents)
		{
			string filePath = Application.persistentDataPath + "/data.txt";

			try
			{
				string jsonData = JsonUtility.ToJson(new WalletComponentListWrapper(walletComponents), true);
				using (StreamWriter writer = new StreamWriter(filePath))
				{
					await writer.WriteAsync(jsonData);
				}
			}
			catch (IOException ex)
			{
				// Handle IOException here
				Debug.LogError("IO error occurred while writing data to file: " + ex.Message);
				// Optionally, rethrow the exception if necessary
				throw;
			}
			catch (Exception ex)
			{
				// Handle any other exceptions here
				Debug.LogError("Error occurred during writing data to file: " + ex.Message);
				// Optionally, rethrow the exception if necessary
				throw;
			}

			Debug.Log($"Finished saving data in {filePath}");
		}
	}

	[Serializable]
	public class WalletComponentListWrapper
	{
		[SerializeField] public List<WalletComponent> WalletComponents;

		public WalletComponentListWrapper(List<WalletComponent> walletComponents)
		{
			WalletComponents = walletComponents;
		}
	}
}