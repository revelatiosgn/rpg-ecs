using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Gameplay
{
    public class PlayerCharacter : NetworkBehaviour, ISpawned
    {
        [SerializeField] private KCC _kcc;
        public KCC KCC => _kcc;

        [SerializeField] private Animator _animator;

        [SerializeField][Range(0f, 100f)] private float _speed = 5f;
        public float Speed => _speed;

	    [SerializeField][Range(0f, 100f)] private float _lookTurnRate = 1.5f;
        public float LookTurnRate => _lookTurnRate;

        private static PlayerCharacter _local;
        public static PlayerCharacter Local => _local;

        [Networked(OnChanged = nameof(OnChangedSceneIndex))]
        public int SceneIndex { get; set; }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                SceneIndex = 2; // "Map_0" todo: get from settings
            }

            if (HasInputAuthority)
            {
                _local = this;
            }
        }

        private static void OnChangedSceneIndex(Changed<PlayerCharacter> changed)
        {
            if (changed.Behaviour.HasStateAuthority || !changed.Behaviour.HasInputAuthority)
                return;

            int newIndex = changed.Behaviour.SceneIndex;
            changed.LoadOld();
            int oldIndex = changed.Behaviour.SceneIndex;

            string pathToScene = SceneUtility.GetScenePathByBuildIndex(newIndex);
            string newSceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
            pathToScene = SceneUtility.GetScenePathByBuildIndex(oldIndex);
            string oldSceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);

            if (oldIndex != 0)
                NetworkManager.Instance.AdditiveSceneLoader.UnloadScene(oldSceneName);
            NetworkManager.Instance.AdditiveSceneLoader.LoadScene(newSceneName);
        }
    }
}

