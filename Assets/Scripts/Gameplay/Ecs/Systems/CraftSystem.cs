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

        private int _noneHash = Animator.StringToHash("None");
        private int _craftHash = Animator.StringToHash("Craft");

        public void Run(EcsSystems systems)
        {
            foreach (OnInteractionEnd onInteractionEnd in EcsManager.EventBus.GetEvents<OnInteractionEnd>())
            {
                int source = onInteractionEnd.SourceEntity;
                int target = onInteractionEnd.TargetEntity;

                if (_crafterPool.Value.Has(source))
                {
                    ref CrafterData crafterData = ref _crafterPool.Value.Get(source);
                    crafterData.Crafter.CraftRecipeId = string.Empty;
                    crafterData.Crafter.WorkbenchId = "0";
                    _crafterPool.Value.Del(source);
                    
                    CharacterAnimation characterAnimation = _animationPool.Value.Get(source).CharacterAnimation;
                    characterAnimation.PlayAnimation(_noneHash, CharacterAnimation.AnimatorLayer.Labor);
                    characterAnimation.SetLayerWeight(0f, CharacterAnimation.AnimatorLayer.Labor);
                }
            }

            foreach (OnInteractionBegin onInteractionBegin in EcsManager.EventBus.GetEvents<OnInteractionBegin>())
            {
                int source = onInteractionBegin.SourceEntity;
                int target = onInteractionBegin.TargetEntity;

                if (_workbenchPool.Value.Has(target))
                {
                    ref CrafterData crafterData = ref _crafterPool.Value.Add(source);
                    crafterData.Crafter = _interactorPool.Value.Get(source).Interactor.GetComponent<Crafter>();
                    crafterData.Crafter.CraftProgress = 0f;
                    crafterData.CraftRate = 1f;
                    crafterData.IsCrafting = false;
                    
                    ref WorkbenchData workbenchData = ref _workbenchPool.Value.Get(target);
                    crafterData.Crafter.WorkbenchId = workbenchData.Workbench.WorkbenchConfig.ID;

                    CharacterAnimation characterAnimation = _animationPool.Value.Get(source).CharacterAnimation;
                    characterAnimation.PlayAnimation(_craftHash, CharacterAnimation.AnimatorLayer.Labor);
                    characterAnimation.SetLayerWeight(1f, CharacterAnimation.AnimatorLayer.Labor);
                }
            }

            foreach (int crafterEntity in _crafterFilter.Value)
            {
                ref CrafterData crafterData = ref _crafterPool.Value.Get(crafterEntity);
                if (string.IsNullOrEmpty(crafterData.Crafter.CraftRecipeId))
                    continue;

                CraftRecipeConfig recipeConfig = crafterData.Crafter.CraftRecipes.GetItem(crafterData.Crafter.CraftRecipeId);
                NetworkDictionary<NetworkString<_32>, int> inventoryItems = _inventoryPool.Value.Get(crafterEntity).Inventory.Items;

                if (!crafterData.IsCrafting)
                {
                    foreach (CraftRecipeConfig.RecipeItem recipeItem in recipeConfig.Requires)
                    {
                        inventoryItems.TryGet(recipeItem.Config.ID, out int count);
                        count = Mathf.Max(count - recipeItem.Count, 0);
                        if (count == 0)
                            inventoryItems.Remove(recipeItem.Config.ID);
                        else
                            inventoryItems.Set(recipeItem.Config.ID, count);
                    }

                    crafterData.IsCrafting = true;

                    Debug.Log($"Start craft {recipeConfig.Result.Config.Name}");
                }

                if (crafterData.IsCrafting)
                    crafterData.Crafter.CraftProgress = crafterData.Crafter.CraftProgress + crafterData.CraftRate * EcsManager.DeltaTime;

                if (crafterData.IsCrafting && crafterData.Crafter.CraftProgress >= 1f)
                {
                    crafterData.Crafter.CraftProgress = 0f;
                    crafterData.Crafter.CraftRecipeId = string.Empty;
                    ItemConfig craftedItem = recipeConfig.Result.Config;

                    inventoryItems.TryGet(craftedItem.ID, out int count);
                    count += recipeConfig.Result.Count;
                    inventoryItems.Set(craftedItem.ID, count);

                    Debug.Log($"Crafted {craftedItem.Name}");
                }
            }
        }
    }
}


