using System.Collections;
using System.Collections.Generic;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class CraftSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CrafterData, InventoryData>> _harvesterFilter = default;
        
        private readonly EcsPoolInject<CrafterData> _crafterPool = default;
        private readonly EcsPoolInject<WorkbenchData> _workbenchPool = default;
        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<InventoryData> _inventoryPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;

        private int _craftHash = Animator.StringToHash("Craft");

        public void Run(EcsSystems systems)
        {
            foreach (OnInteractBegin onInteractBegin in EcsManager.EventBus.GetEvents<OnInteractBegin>())
            {
                int interactor = onInteractBegin.InteractorEntity;
                int interactable = onInteractBegin.InteractableEntity;

                if (_workbenchPool.Value.Has(interactable))
                {
                    Debug.Log("add crafter");

                    ref CrafterData crafterData = ref _crafterPool.Value.Add(interactor);
                    crafterData.Crafter = _interactorPool.Value.Get(interactor).Interactor.GetComponent<Crafter>();
                    
                    ref WorkbenchData workbenchData = ref _workbenchPool.Value.Get(interactable);
                    crafterData.Crafter.WorkbenchId = workbenchData.Workbench.WorkbenchConfig.ID;
                    // harvesterData.TargetEntity = interactable;
                    // harvesterData.HarvestProgress = 0f;
                    // harvesterData.HarvestRate = 1f;

                    _animationPool.Value.Get(interactor).CharacterAnimation.PlayAnimation(_craftHash);
                }
            }

            foreach (OnInteractEnd onInteractEnd in EcsManager.EventBus.GetEvents<OnInteractEnd>())
            {
                int interactor = onInteractEnd.InteractorEntity;
                int interactable = onInteractEnd.InteractableEntity;

                if (_crafterPool.Value.Has(interactor))
                {
                    Debug.Log("del crafter");
                    ref CrafterData crafterData = ref _crafterPool.Value.Get(interactor);
                    crafterData.Crafter.WorkbenchId = "0";
                    _crafterPool.Value.Del(interactor);
                }
            }

            // foreach (int harvesterEntity in _harvesterFilter.Value)
            // {
            //     ref HarvesterData harvesterData = ref _harvesterPool.Value.Get(harvesterEntity);
            //     ref HarvestableData harvestableData = ref _harvestablePool.Value.Get(harvesterData.TargetEntity);

            //     harvesterData.HarvestProgress += harvesterData.HarvestRate * EcsManager.NetworkRunner.DeltaTime;

            //     if (harvesterData.HarvestProgress >= 1f)
            //     {
            //         harvesterData.HarvestProgress = 0f;
            //         InventoryItemConfig itemConfig = harvestableData.Harvestable.HarvestItem;

            //         NetworkDictionary<NetworkString<_32>, int> items = _inventoryPool.Value.Get(harvesterEntity).Inventory.Items;

            //         items.TryGet(itemConfig.ID, out int count);
            //         count++;
            //         items.Set(itemConfig.ID, count);
            //     }
            // }
        }
    }
}


