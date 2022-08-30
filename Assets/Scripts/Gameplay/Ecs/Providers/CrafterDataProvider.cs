using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class CrafterDataProvider : MonoProvider<CrafterData>
    {
        [SerializeField] private Crafter _crafter;

        private void Awake()
        {
            value.Crafter = _crafter;
        }
    }
}


