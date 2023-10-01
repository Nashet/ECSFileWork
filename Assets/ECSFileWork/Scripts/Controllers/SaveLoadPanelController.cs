using Nashet.ECSFileWork.ECS;
using Nashet.ECSFileWork.Views;
using System;
using Unity.Collections;
using Unity.Entities;

namespace Nashet.ECSFileWork.Controllers
{
	using System.Collections;
	using UnityEngine;
	public class SaveLoadPanelController : MonoBehaviour
	{
		[SerializeField] private SaveLoadPanelView saveLoadPanelView;
		private EntityManager entityManager;

		private IEnumerator Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			saveLoadPanelView.OnSaveClicked += SaveClickedHandler;
			saveLoadPanelView.OnLoadClicked += LoadClickedHandler;

			yield return new WaitForSeconds(0.5f); // Delay is neded for other systems to set up. In real task it should be done in a right way

			LoadData();
		}

		private void LoadClickedHandler()
		{
			LoadData();
		}

		private void LoadData()
		{
			AddComponent(new LoadFlagComponent());
			//todo maybe add UI blocking until data is actually loaded. Overwise user might change UI while data loading is in progress
		}

		private void SaveClickedHandler()
		{
			AddComponent(new SaveFlagComponent()); //in real task you probably will load all data at once
		}

		private void AddComponent<T>(T componentData) where T : unmanaged, IComponentData
		{
			// Query the entity with the desired field value
			EntityQuery query = entityManager.CreateEntityQuery(
				ComponentType.ReadOnly<WalletComponent>()
			);

			NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
			for (int i = 0; i < entities.Length; i++)
			{
				Entity entity = entities[i];
				entityManager.AddComponentData(entity, componentData);
			}
		}
	}
}