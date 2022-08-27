using System.Collections;
using System.Collections.Generic;
using Fusion;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using RPGGame.Model;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public class EcsManager : NetworkBehaviour
    {
        private EcsWorld _world;
        private EcsSystems _updateSystems;
        private EcsSystems _fixedUpdateSystems;
        private EcsSystems _networkSystems;
        private EcsSharedData _sharedData;

        private void Start()
        {
            if (NetworkManager.Instance.NetworkRunner.IsServer)
                Setup();
        }

        private void Setup()
        {
            _world = new EcsWorld();
            _sharedData = new EcsSharedData();

            _updateSystems = new EcsSystems(_world, _sharedData);
            _fixedUpdateSystems = new EcsSystems(_world, _sharedData);
            _networkSystems = new EcsSystems(_world, _sharedData);

            _updateSystems.ConvertScene();

            // _networkSystems
            //     .Inject()
            //     .Init();
        }

        public override void FixedUpdateNetwork()
        {
            if (NetworkManager.Instance.NetworkRunner.IsServer)
                _networkSystems?.Run();
        }

        private void OnDestroy()
        {
            _updateSystems?.Destroy();
            _fixedUpdateSystems?.Destroy();
            _networkSystems?.Destroy();

            _world?.Destroy();
        }
    }
}


