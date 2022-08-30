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
    public class WorkbenchConfig : ScriptableObject
    {
        [UniqueIdentifier][SerializeField] string _id;
        public string ID => _id;

        [SerializeField] private string _name;
        public string Name => _name;
    }
}

