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

        private int _noneHash = Animator.StringToHash("None");
        private int _harvestHash = Animator.StringToHash("Harvest");

        public void Run(EcsSystems systems)
        {
            foreach (OnInteractionEnd onInteractionEnd in EcsManager.EventBus.GetEvents<OnInteractionEnd>())
            {
                int source = onInteractionEnd.SourceEntity;
                int target = onInteractionEnd.TargetEntity;

                if (_harvesterPool.Value.Has(source))
                    _harvesterPool.Value.Del(source);

                CharacterAnimation characterAnimation = _animationPool.Value.Get(source).CharacterAnimation;
                characterAnimation.PlayAnimation(_noneHash, CharacterAnimation.AnimatorLayer.Labor);
                characterAnimation.SetLayerWeight(0f, CharacterAnimation.AnimatorLayer.Labor);
            }

            foreach (OnInteractionBegin onInteractionBegin in EcsManager.EventBus.GetEvents<OnInteractionBegin>())
            {
                int source = onInteractionBegin.SourceEntity;
                int target = onInteractionBegin.TargetEntity;

                if (_harvestablePool.Value.Has(target))
                {
                    ref HarvesterData harvesterData = ref _harvesterPool.Value.Add(source);
                    harvesterData.TargetEntity = target;
                    harvesterData.HarvestProgress = 0f;
                    harvesterData.HarvestRate = 1f;

                    CharacterAnimation characterAnimation = _animationPool.Value.Get(source).CharacterAnimation;
                    characterAnimation.PlayAnimation(_harvestHash, CharacterAnimation.AnimatorLayer.Labor);
                    characterAnimation.SetLayerWeight(1f, CharacterAnimation.AnimatorLayer.Labor);
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

                    Debug.Log($"Harvested {itemConfig.Name}");
                }
            }
        }
    }
}


