using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class HarvesterDataProvider : MonoProvider<HarvesterData>
    {
        [SerializeField, Min(0f)] private float _harvestRate = 1f;

        private void Awake()
        {
            value.HarvestRate = _harvestRate;
            value.HarvestProgress = 0f;
        }
    }
}


