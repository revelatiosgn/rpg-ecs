using System.Collections;
using System.Collections.Generic;
using Example;
using Fusion.KCC;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class PlayerInputDataProvider : MonoProvider<PlayerInputData>
    {
        [SerializeField] private PlayerInput _playerInput;

        private void Awake()
        {
            value.PlayerInput = _playerInput;
        }
    }
}


