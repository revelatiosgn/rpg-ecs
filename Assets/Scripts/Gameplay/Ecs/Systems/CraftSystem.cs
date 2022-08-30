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
        private readonly EcsFilterInject<Inc<CrafterData, InventoryData>> _crafterFilter = default;
        
        private readonly EcsPoolInject<CrafterData> _crafterPool = default;
        private readonly EcsPoolInject<WorkbenchData> _workbenchPool = default;
        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<InventoryData> _inventoryPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;

        private int _craftHash = Animator.StringToHash("Craft");

        public void Run(EcsSystems systems)
        {
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
                    crafterData.CraftRate = 1f;
                    crafterData.Crafter.CraftProgress = 0f;

                    _animationPool.Value.Get(interactor).CharacterAnimation.PlayAnimation(_craftHash);
                }
            }

            foreach (int crafterEntity in _crafterFilter.Value)
            {
                ref CrafterData crafterData = ref _crafterPool.Value.Get(crafterEntity);
                if (string.IsNullOrEmpty(crafterData.Crafter.IntendRecipeId))
                    continue;

                CraftRecipeConfig recipeConfig = crafterData.Crafter.CraftRecipes.GetItem(crafterData.Crafter.IntendRecipeId);
                NetworkDictionary<NetworkString<_32>, int> inventoryItems = _inventoryPool.Value.Get(crafterEntity).Inventory.Items;

                if (crafterData.Crafter.CraftProgress == 0f)
                {
                    Debug.Log($"Start craft! {recipeConfig.Result.Config.Name}");

                    foreach (CraftRecipeConfig.RecipeItem recipeItem in recipeConfig.Requires)
                    {
                        inventoryItems.TryGet(recipeItem.Config.ID, out int count);
                        count = Mathf.Max(count - recipeItem.Count, 0);
                        if (count == 0)
                            inventoryItems.Remove(recipeItem.Config.ID);
                        else
                            inventoryItems.Set(recipeItem.Config.ID, count);
                    }
                }

                crafterData.Crafter.CraftProgress = crafterData.Crafter.CraftProgress + crafterData.CraftRate * EcsManager.DeltaTime;

                if (crafterData.Crafter.CraftProgress >= 1f)
                {
                    crafterData.Crafter.CraftProgress = 0f;
                    crafterData.Crafter.IntendRecipeId = string.Empty;
                    ItemConfig craftedItem = recipeConfig.Result.Config;
                    Debug.Log($"Crafted! {craftedItem.Name}");

                    inventoryItems.TryGet(craftedItem.ID, out int count);
                    count += recipeConfig.Result.Count;
                    inventoryItems.Set(craftedItem.ID, count);
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


