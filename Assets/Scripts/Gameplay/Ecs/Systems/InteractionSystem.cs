using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public class InteractionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<InteractorData, TransformData, KCCData, AnimationData>> _interactorFilter = default;

        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<TransformData> _transformPool = default;
        private readonly EcsPoolInject<KCCData> _kccPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;
        
        private int _moveHash = Animator.StringToHash("Move");

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _interactorFilter.Value)
            {
                ref InteractorData interactorData = ref _interactorPool.Value.Get(entity);
                ref TransformData transformData = ref _transformPool.Value.Get(entity);
                ref KCCData kccData = ref _kccPool.Value.Get(entity);
                ref AnimationData animationData = ref _animationPool.Value.Get(entity);

                Interactable intendedInteractable = interactorData.Interactor.IntendedInteractable;
                if (intendedInteractable != null)
                {
                    if (kccData.KCC.RenderData.RealSpeed > float.Epsilon)
                    {
                        interactorData.Interactor.IntendedInteractable = null;
                    }
                    else if (intendedInteractable.Object.Id != interactorData.Interactor.TargetInteractableId)
                    {
                        if (interactorData.Interactor.Interactables.TryGet(intendedInteractable.Object.Id, out Interactable newTarget))
                        {
                            Interactable oldTarget = interactorData.Interactor.TargetInteractable;

                            if (TryGetEntity(oldTarget, out int oldEntity))
                            {
                                // end interact
                                interactorData.Interactor.IntendedInteractable = null;
                                interactorData.Interactor.TargetInteractable = null;
                                interactorData.Interactor.TargetInteractableId = default;
                                EcsManager.EventBus.RaiseEvent<OnInteractEnd>(new OnInteractEnd { InteractorEntity = entity, InteractableEntity = oldEntity });

                                _animationPool.Value.Get(entity).CharacterAnimation.PlayAnimation(_moveHash);
                            }

                            if (TryGetEntity(newTarget, out int newEntity))
                            {
                                // begin interact
                                interactorData.Interactor.IntendedInteractable = null;
                                interactorData.Interactor.TargetInteractable = newTarget;
                                interactorData.Interactor.TargetInteractableId = newTarget.Object.Id;
                                EcsManager.EventBus.RaiseEvent<OnInteractBegin>(new OnInteractBegin { InteractorEntity = entity, InteractableEntity = newEntity });
                            }
                        }
                    }
                }

                Interactable targetInteractable = interactorData.Interactor.TargetInteractable;
                if (targetInteractable != null)
                {
                    if (kccData.KCC.RenderData.RealSpeed > float.Epsilon)
                    {
                        Debug.Log("Start moving");
                        Interactable oldTarget = interactorData.Interactor.TargetInteractable;
                        if (TryGetEntity(oldTarget, out int oldEntity))
                        {
                            Debug.Log("End interact");
                            // end interact
                            interactorData.Interactor.IntendedInteractable = null;
                            interactorData.Interactor.TargetInteractable = null;
                            interactorData.Interactor.TargetInteractableId = default;
                            EcsManager.EventBus.RaiseEvent<OnInteractEnd>(new OnInteractEnd { InteractorEntity = entity, InteractableEntity = oldEntity });

                            _animationPool.Value.Get(entity).CharacterAnimation.PlayAnimation(_moveHash);
                        }
                    }
                    else
                    {
                        Vector3 direction = targetInteractable.transform.position - transformData.Transform.position;
                        kccData.KCC.SetLookRotation(Quaternion.LookRotation(direction, Vector3.up));
                    }
                }
            }
        }

        private bool TryGetEntity(MonoBehaviour monoBehaviour, out int entity)
        {
            entity = -1;

            if (monoBehaviour == null)
                return false;

            if (monoBehaviour.TryGetComponent<ConvertToEntity>(out ConvertToEntity convertToEntity))
            {
                if (convertToEntity.TryGetEntity(out int result, out EcsWorld world))
                {
                    entity = result;
                    return true;
                }
            }

            return false;
        }
    }
}

/*
if (target != null)
                {
                    if (kccData.KCC.RenderData.RealSpeed > float.Epsilon)
                    {
                        interactorData.Interactor.ResetTarget();
                        animationData.CharacterAnimation.SetAnimation(_moveHash);
                        return;
                    }

                    if (interactorData.Interactor.CurrentInteractable == default)
                    {
                        interactorData.Interactor.CurrentInteractable = target.Object.Id;
                        interactorData.Harvested = 0f;
                    }
                    else
                    {
                        interactorData.Harvested += Time.deltaTime;
                        if (interactorData.Harvested >= 3f)
                        {
                            // Debug.Log("HARVESTED!");
                            EcsManager.EventBus.RaiseEvent<TestEvent>(new TestEvent { ival = 3, message = "harvested" });
                            interactorData.Harvested = 0f;
                        }
                    }

                    animationData.CharacterAnimation.SetAnimation(_slashHash);
                    Vector3 direction = target.transform.position - transformData.Transform.position;
                    kccData.KCC.SetLookRotation(Quaternion.LookRotation(direction, Vector3.up));
                }
                else
                {
                    animationData.CharacterAnimation.SetAnimation(_moveHash);
                }
                */


