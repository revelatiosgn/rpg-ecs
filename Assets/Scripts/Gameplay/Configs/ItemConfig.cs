using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public abstract class ItemConfig : ScriptableObject
    {
        [UniqueIdentifier][SerializeField] string _id;
        public string ID => _id;

        [SerializeField] private string _name;
        public string Name => _name;
    }
}
