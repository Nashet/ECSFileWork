using Nashet.ECSFileWork.ECS;
using Nashet.ECSFileWork.Views;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nashet.ECSFileWork.Controllers
{
	public class SaveLoadPanelController : MonoBehaviour
	{
		[SerializeField] private SaveLoadPanelView saveLoadPanelView;
		private EntityManager entityManager;

		private void Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

			saveLoadPanelView.OnSaveClicked += SaveClickedHandler;
			saveLoadPanelView.OnLoadClicked += LoadClickedHandler;
		}

		private void LoadClickedHandler()
		{
			throw new NotImplementedException();
		}

		private void SaveClickedHandler()
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
				entityManager.AddComponentData(entity, new SaveFlagComponent());
			}
		}
	}
}