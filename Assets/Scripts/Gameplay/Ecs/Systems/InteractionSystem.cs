using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class InteractionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<InteractorData, TransformData, KCCData, AnimationData>> _filter = default;

        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<TransformData> _transformPool = default;
        private readonly EcsPoolInject<KCCData> _kccPool = default;
        private readonly EcsPoolInject<AnimationData> _animationPool = default;
        
        private int _moveHash = Animator.StringToHash("Move");
        private int _slashHash = Animator.StringToHash("Slash");

        public void Run(EcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref InteractorData interactorData = ref _interactorPool.Value.Get(entity);
                ref TransformData transformData = ref _transformPool.Value.Get(entity);
                ref KCCData kccData = ref _kccPool.Value.Get(entity);
                ref AnimationData animationData = ref _animationPool.Value.Get(entity);

                Interactable target = interactorData.Interactor.InteractTarget;

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
            }
        }
    }
}


