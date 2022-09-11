using System.Collections;
using System.Collections.Generic;
using Example;
using Fusion.KCC;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class PlayerControllerDataProvider : MonoProvider<PlayerControllerData>
    {
        [SerializeField] private ThirdPersonPlayerController _playerController;

        private void Awake()
        {
            value.PlayerController = _playerController;
        }
    }
}


