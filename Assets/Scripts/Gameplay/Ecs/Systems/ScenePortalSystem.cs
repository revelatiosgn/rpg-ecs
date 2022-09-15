using System.Collections;
using System.Collections.Generic;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Gameplay.Ecs
{
    public class ScenePortalSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<ScenePortalData> _scenePortalPool = default;
        private readonly EcsPoolInject<InteractorData> _interactorPool = default;
        private readonly EcsPoolInject<KCCData> _kccPool = default;

        public void Run(EcsSystems systems)
        {
            foreach (OnInteractionBegin onInteractionBegin in EcsManager.EventBus.GetEvents<OnInteractionBegin>())
            {
                int source = onInteractionBegin.SourceEntity;
                int target = onInteractionBegin.TargetEntity;

                if (_scenePortalPool.Value.Has(target) && _interactorPool.Value.Has(source))
                {
                    Debug.Log($"{_interactorPool.Value.Get(source).Interactor.name} use {_scenePortalPool.Value.Get(target).ScenePortal.name}");

                    PlayerCharacter playerCharacter = _interactorPool.Value.Get(source).Interactor.GetComponent<PlayerCharacter>();

                    ScenePortal sourcePortal = _scenePortalPool.Value.Get(target).ScenePortal;
                    ScenePortal targetPortal = ScenePortal.GetPortal(sourcePortal.TargetPortal);

                    playerCharacter.SceneIndex = SceneManager.GetSceneByName(targetPortal.gameObject.scene.name).buildIndex;
                    _kccPool.Value.Get(source).KCC.SetPosition(targetPortal.transform.position);
                }
            }
        }
    }
}


