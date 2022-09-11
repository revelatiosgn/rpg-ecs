using System.Collections;
using System.Collections.Generic;
using Example;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public class InteractionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<InteractorData, TransformData, PlayerControllerData, AnimationData>> _interactorFilter = default;

        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<InteractableData> _interactablePool = default;
        private readonly EcsPoolInject<TransformData> _transformPool = default;
        private readonly EcsPoolInject<PlayerControllerData> _playerControllerPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;
        
        private int _moveHash = Animator.StringToHash("Move");

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _interactorFilter.Value)
            {
                ref InteractorData source = ref _interactorPool.Value.Get(entity);

                // End interact by move
                if (source.Interactor.Target != null)
                {
                    ref PlayerControllerData playerControllerData = ref _playerControllerPool.Value.Get(entity);
                    if (playerControllerData.PlayerController.Input.FixedInput.MoveDirection != Vector2.zero)
                    {
                        EcsManager.EventBus.RaiseEvent<OnInteractionEnd>(new OnInteractionEnd { 
                            SourceEntity = entity,
                            TargetEntity = source.Interactor.Target.GetComponent<EntityObject>().Id
                        });

                        source.Interactor.Target = null;
                        _animationPool.Value.Get(entity).CharacterAnimation.PlayAnimation(_moveHash);
                    }
                }

                // Look at target
                if (source.Interactor.Target != null)
                {
                    ref TransformData transformData = ref _transformPool.Value.Get(entity);
                    transformData.Transform.LookAt(source.Interactor.Target.transform, Vector3.up);
                }
            }
            
            // End interact by player
            foreach (OnPlayerInterruptInteract onPlayerInterruptInteract in EcsManager.EventBus.GetEvents<OnPlayerInterruptInteract>())
            {
                ref InteractorData source = ref _interactorPool.Value.Get(onPlayerInterruptInteract.SourceEntity);
                ref InteractableData target = ref _interactablePool.Value.Get(onPlayerInterruptInteract.TargetEntity);

                if (source.Interactor.Target != null)
                {
                    EcsManager.EventBus.RaiseEvent<OnInteractionEnd>(new OnInteractionEnd { 
                        SourceEntity = onPlayerInterruptInteract.SourceEntity,
                        TargetEntity = source.Interactor.Target.GetComponent<EntityObject>().Id
                    });
                }

                source.Interactor.Target = null;
                _animationPool.Value.Get(onPlayerInterruptInteract.SourceEntity).CharacterAnimation.PlayAnimation(_moveHash);
            }
            
            // Begin interact
            foreach (OnPlayerInteract onPlayerInteract in EcsManager.EventBus.GetEvents<OnPlayerInteract>())
            {
                ref InteractorData source = ref _interactorPool.Value.Get(onPlayerInteract.SourceEntity);
                ref InteractableData target = ref _interactablePool.Value.Get(onPlayerInteract.TargetEntity);

                if (!source.Interactor.Interactables.ContainsKey(target.Interactable.Object.Id))
                    continue;

                ref PlayerControllerData playerControllerData = ref _playerControllerPool.Value.Get(onPlayerInteract.SourceEntity);
                if (playerControllerData.PlayerController.Input.FixedInput.MoveDirection != Vector2.zero)
                    continue;

                Debug.Log($"OnPlayerInteract {source.Interactor.name} with {target.Interactable.name}");

                if (source.Interactor.Target != null)
                {
                    EcsManager.EventBus.RaiseEvent<OnInteractionEnd>(new OnInteractionEnd { 
                        SourceEntity = onPlayerInteract.SourceEntity,
                        TargetEntity = source.Interactor.Target.GetComponent<EntityObject>().Id
                    });
                }

                source.Interactor.Target = target.Interactable;
                EcsManager.EventBus.RaiseEvent<OnInteractionBegin>(new OnInteractionBegin { 
                    SourceEntity = onPlayerInteract.SourceEntity,
                    TargetEntity = onPlayerInteract.TargetEntity 
                });
            }
        }
    }
}