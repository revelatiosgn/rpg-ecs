using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using TMPro;
using UnityEngine;

namespace RPGGame.Gameplay
{
    [CreateAssetMenu(menuName = "RPG/Craft/WorkbenchConfig")]
    public class WorkbenchConfig : ItemConfig
    {
        [SerializeField] private List<CraftRecipeConfig> _recipes;
        public List<CraftRecipeConfig> Recipes => _recipes;
    }
}

