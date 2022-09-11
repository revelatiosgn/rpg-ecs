using System.Collections;
using System.Collections.Generic;
using Example;
using Fusion.KCC;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class PlayerBehaviourDataProvider : MonoProvider<PlayerBehaviourData>
    {
        [SerializeField] private PlayerBehaviour _playerBehaviour;

        private void Awake()
        {
            value.PlayerBehaviour = _playerBehaviour;
        }
    }
}


