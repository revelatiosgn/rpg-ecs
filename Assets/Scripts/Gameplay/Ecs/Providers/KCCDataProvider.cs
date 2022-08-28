using System.Collections;
using System.Collections.Generic;
using Fusion.KCC;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class KCCDataProvider : MonoProvider<KCCData>
    {
        [SerializeField] private KCC _kcc;

        private void Awake()
        {
            value.KCC = _kcc;
        }
    }
}


