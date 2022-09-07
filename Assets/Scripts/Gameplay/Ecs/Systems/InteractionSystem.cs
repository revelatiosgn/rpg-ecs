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
        private readonly EcsFilterInject<Inc<InteractorData, TransformData, PlayerInputData, AnimationData>> _interactorFilter = default;

        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<InteractableData> _interactablePool = default;
        private readonly EcsPoolInject<TransformData> _transformPool = default;
        private readonly EcsPoolInject<PlayerInputData> _playerInputPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;
        
        private int _moveHash = Animator.StringToHash("Move");

        public void Run(EcsSystems systems)
        {
            // End interact by move
            foreach (int entity in _interactorFilter.Value)
            {
                ref InteractorData source = ref _interactorPool.Value.Get(entity);
                if (source.Interactor.Target != null)
                {
                    ref PlayerInputData playerInputData = ref _playerInputPool.Value.Get(entity);
                    if (playerInputData.PlayerInput.FixedInput.MoveDirection != Vector2.zero)
                    {
                        EcsManager.EventBus.RaiseEvent<OnInteractionEnd>(new OnInteractionEnd { 
                            SourceEntity = entity,
                            TargetEntity = source.Interactor.Target.GetComponent<EntityObject>().Id
                        });

                        source.Interactor.Target = null;
                        _animationPool.Value.Get(entity).CharacterAnimation.PlayAnimation(_moveHash);
                    }
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

                ref PlayerInputData playerInputData = ref _playerInputPool.Value.Get(onPlayerInteract.SourceEntity);
                if (playerInputData.PlayerInput.FixedInput.MoveDirection != Vector2.zero)
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