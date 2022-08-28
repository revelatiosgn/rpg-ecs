using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class TransformDataProvider : MonoProvider<TransformData>
    {
        [SerializeField] private Transform _transform;

        private void Awake()
        {
            value.Transform = _transform;
        }
    }
}


