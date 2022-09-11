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
    public class PlayerCharacter : NetworkBehaviour
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

        private static void OnChangedSceneIndex(Changed<PlayerCharacter> changed)
        {
            Debug.Log($"OnChangedSceneIndex {changed.Behaviour.SceneIndex}");
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                _local = this;
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RPC_LoadScene(string sceneName)
        {
            if (!HasStateAuthority)
                NetworkManager.Instance.AdditiveSceneLoader.LoadScene(sceneName);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RPC_UnloadScene(string sceneName)
        {
            if (!HasStateAuthority)
                NetworkManager.Instance.AdditiveSceneLoader.UnloadScene(sceneName);
        }
    }
}

