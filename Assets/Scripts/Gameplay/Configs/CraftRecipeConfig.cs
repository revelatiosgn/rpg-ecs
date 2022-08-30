using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using TMPro;
using UnityEngine;

namespace RPGGame.Gameplay
{
    [CreateAssetMenu(menuName = "RPG/Craft/RecipeConfig")]
    public class CraftRecipeConfig : ItemConfig
    {
        [SerializeField] private List<RecipeItem> _requires;
        public List<RecipeItem> Requires => _requires;

        [SerializeField] private RecipeItem _result;
        public RecipeItem Result => _result;

        [Serializable]
        public struct RecipeItem
        {
            public InventoryItemConfig Config;
            public int Count;
        }
    }
}

