using System.Collections;
using System.Collections.Generic;
using Example;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class PlayerBehaviourSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<PlayerBehaviourData> _playerBehaviourPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;
        private readonly EcsPoolInject<PlayerControllerData> _playerControllerPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (OnPlayerSwitchBehaviour onPlayerSwitchBehaviour in EcsManager.EventBus.GetEvents<OnPlayerSwitchBehaviour>())
            {
                int entity = onPlayerSwitchBehaviour.Entity;

                ref PlayerBehaviourData playerBehaviourData = ref _playerBehaviourPool.Value.Get(entity);
                playerBehaviourData.PlayerBehaviour.BehaviourState = onPlayerSwitchBehaviour.BehaviourState;

                ref AnimationData animationData = ref _animationPool.Value.Get(entity);
                animationData.CharacterAnimation.Behaviour = 
                    playerBehaviourData.PlayerBehaviour.BehaviourState == PlayerBehaviour.PlayerBehaviourState.Labor ? 0f : 1f;

                ref PlayerControllerData playerControllerData = ref _playerControllerPool.Value.Get(entity);
                playerControllerData.PlayerController.Rotation = playerBehaviourData.PlayerBehaviour.BehaviourState == PlayerBehaviour.PlayerBehaviourState.Labor ?
                    ThirdPersonPlayerController.RotationMode.RotateTowardsMove : 
                    Example.ThirdPersonPlayerController.RotationMode.RotateTowardsCursor;
            }
        }
    }
}


