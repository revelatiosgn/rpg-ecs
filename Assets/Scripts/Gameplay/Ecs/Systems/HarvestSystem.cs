using System.Collections;
using System.Collections.Generic;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class HarvestSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<HarvesterData, InventoryData>> _harvesterFilter = default;
        
        private readonly EcsPoolInject<HarvesterData> _harvesterPool = default;
        private readonly EcsPoolInject<InventoryData> _inventoryPool = default;
        private readonly EcsPoolInject<HarvestableData> _harvestablePool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;

        private int _slashHash = Animator.StringToHash("Slash");

        public void Run(EcsSystems systems)
        {
            foreach (OnInteractBegin onInteractBegin in EcsManager.EventBus.GetEvents<OnInteractBegin>())
            {
                int interactor = onInteractBegin.InteractorEntity;
                int interactable = onInteractBegin.InteractableEntity;

                if (_harvestablePool.Value.Has(interactable))
                {
                    Debug.Log("add harvester");
                    ref HarvesterData harvesterData = ref _harvesterPool.Value.Add(interactor);
                    harvesterData.TargetEntity = interactable;
                    harvesterData.HarvestProgress = 0f;
                    harvesterData.HarvestRate = 1f;

                    _animationPool.Value.Get(interactor).CharacterAnimation.PlayAnimation(_slashHash);
                }
            }

            foreach (OnInteractEnd onInteractEnd in EcsManager.EventBus.GetEvents<OnInteractEnd>())
            {
                int interactor = onInteractEnd.InteractorEntity;
                int interactable = onInteractEnd.InteractableEntity;

                if (_harvesterPool.Value.Has(interactor))
                {
                    Debug.Log("del harvester");
                    _harvesterPool.Value.Del(interactor);
                }
            }

            foreach (int harvesterEntity in _harvesterFilter.Value)
            {
                ref HarvesterData harvesterData = ref _harvesterPool.Value.Get(harvesterEntity);
                ref HarvestableData harvestableData = ref _harvestablePool.Value.Get(harvesterData.TargetEntity);

                harvesterData.HarvestProgress += harvesterData.HarvestRate * EcsManager.NetworkRunner.DeltaTime;

                if (harvesterData.HarvestProgress >= 1f)
                {
                    harvesterData.HarvestProgress = 0f;
                    InventoryItemConfig itemConfig = harvestableData.Harvestable.HarvestItem;

                    NetworkDictionary<NetworkString<_32>, int> items = _inventoryPool.Value.Get(harvesterEntity).Inventory.Items;

                    items.TryGet(itemConfig.ID, out int count);
                    count++;
                    items.Set(itemConfig.ID, count);
                }
            }
        }
    }
}


