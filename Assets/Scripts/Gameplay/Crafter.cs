using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Gameplay.Events;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Crafter : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnWorkbenchChanged))]
        public NetworkString<_32> WorkbenchId { get; set; }

        [Networked(OnChanged = nameof(OnCraftProgressChanged))]
        public float CraftProgress { get; set; }

        public string IntendRecipeId;
        public string CraftRecipeId;

        [SerializeField] private CraftRecipesConfig _craftRecipes;
        public CraftRecipesConfig CraftRecipes => _craftRecipes;

        [SerializeField] private GameplayEvents _gameplayEvents;

        private static void OnWorkbenchChanged(Changed<Crafter> changed)
        {
            if (changed.Behaviour.HasInputAuthority)
                changed.Behaviour._gameplayEvents.OnWorkbench?.Invoke(changed.Behaviour.WorkbenchId);
        }

        private static void OnCraftProgressChanged(Changed<Crafter> changed)
        {
            if (changed.Behaviour.HasInputAuthority)
                changed.Behaviour._gameplayEvents.OnCraftProgressChanged?.Invoke(changed.Behaviour.CraftProgress);
        }

        public void CraftRecipe(string recipeId)
        {
            if (Object.HasInputAuthority)
                RPC_CraftRecipe(recipeId);
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_CraftRecipe(string recipeId)
        {
            if (Object.HasStateAuthority)
                IntendRecipeId = recipeId;
        }
    }
}

