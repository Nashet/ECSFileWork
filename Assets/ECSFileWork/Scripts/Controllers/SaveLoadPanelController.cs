using Nashet.ECSFileWork.ECS;
using Nashet.ECSFileWork.Views;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Nashet.ECSFileWork.Controllers
{
	public class SaveLoadPanelController : MonoBehaviour
	{
		[SerializeField] private SaveLoadPanelView saveLoadPanelView;
		private EntityManager entityManager;
		private List<List<SystemBase>> availableSaveLoadSystems = new();
		private const string checkLaunchKey = "wasAppLaunched";

		private IEnumerator Start()
		{
			var world = World.DefaultGameObjectInjectionWorld;
			entityManager = world.EntityManager;

			SetAvailableFileSystems(world);

			saveLoadPanelView.OnSaveClicked += SaveClickedHandler;
			saveLoadPanelView.OnLoadClicked += LoadClickedHandler;
			saveLoadPanelView.OnDropdownValueChanged += DropdownValueChangedHandler;
			EnableRightSystem(0);


			yield return new WaitForSeconds(0.5f); // Delay is neded for other systems to set up. In real task it should be done in a right way

			var wasAppLaunched = PlayerPrefs.GetInt(checkLaunchKey);
			if (wasAppLaunched == 0)
			{
				Debug.Log("Creating default empty save file..");
				SaveData(); //creates default record
				PlayerPrefs.SetInt(checkLaunchKey, 1);
				PlayerPrefs.Save();
			}
			else
			{
				LoadData();
			}
		}

		private void OnDestroy()
		{
			saveLoadPanelView.OnSaveClicked -= SaveClickedHandler;
			saveLoadPanelView.OnLoadClicked -= LoadClickedHandler;
			saveLoadPanelView.OnDropdownValueChanged -= DropdownValueChangedHandler;
		}

		private void SetAvailableFileSystems(World world)
		{
			var playerPrefsSystems = new List<SystemBase> {
				world.GetExistingSystemManaged<SaveToPlayerPrefsSystem>(),
				world.GetExistingSystemManaged<LoadFromPlayerprefSystem>(),
			};

			availableSaveLoadSystems.Add(playerPrefsSystems);

			var txtFileSystem = new List<SystemBase>
			{
				world.GetExistingSystemManaged<SaveTxtFileSystem>(),
				world.GetExistingSystemManaged<LoadTxtFileSystem>(),
			};

			availableSaveLoadSystems.Add(txtFileSystem);
		}

		private void DropdownValueChangedHandler(int value)
		{
			EnableRightSystem(value);
		}

		private void EnableRightSystem(int choosedSystem)
		{
			for (int i = 0; i < availableSaveLoadSystems.Count; i++)
			{
				List<SystemBase> set = availableSaveLoadSystems[i];
				foreach (var system in set)
				{
					system.Enabled = i == choosedSystem;
				}
			}
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
			SaveData();
		}

		private void SaveData()
		{
			AddComponent(new SaveFlagComponent()); //in real task you probably will load all data at once
		}

		private void AddComponent<T>(T componentData) where T : unmanaged, IComponentData
		{
			// Query the entity with the desired field value
			EntityQuery query = entityManager.CreateEntityQuery(
				ComponentType.ReadOnly<WalletComponent>()
			);

			using (var entities = query.ToEntityArray(Allocator.TempJob))
			{
				for (int i = 0; i < entities.Length; i++)
				{
					Entity entity = entities[i];
					entityManager.AddComponentData(entity, componentData);
				}
			}
		}
	}
}