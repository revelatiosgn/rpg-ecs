using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class HarvestableDataProvider : MonoProvider<HarvestableData>
    {
        [SerializeField] private Harvestable _harvestable;

        private void Awake()
        {
            value.Harvestable = _harvestable;
        }
    }
}


