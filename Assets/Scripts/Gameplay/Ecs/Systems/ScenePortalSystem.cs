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
            foreach (OnInteractBegin onInteractBegin in EcsManager.EventBus.GetEvents<OnInteractBegin>())
            {
                int interactor = onInteractBegin.InteractorEntity;
                int interactable = onInteractBegin.InteractableEntity;

                if (_scenePortalPool.Value.Has(interactable) && _interactorPool.Value.Has(interactor))
                {
                    Debug.Log($"{_interactorPool.Value.Get(interactor).Interactor.name} use {_scenePortalPool.Value.Get(interactable).ScenePortal.name}");

                    PlayerCharacter playerCharacter = _interactorPool.Value.Get(interactor).Interactor.GetComponent<PlayerCharacter>();

                    ScenePortal sourcePortal = _scenePortalPool.Value.Get(interactable).ScenePortal;
                    ScenePortal targetPortal = ScenePortal.GetPortal(sourcePortal.TargetPortal);

                    playerCharacter.RPC_LoadScene(targetPortal.gameObject.scene.name);
                    _kccPool.Value.Get(interactor).KCC.SetPosition(targetPortal.transform.position);
                    playerCharacter.RPC_UnloadScene(sourcePortal.gameObject.scene.name);
                }
            }
        }
    }
}


