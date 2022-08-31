using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Model
{
    public class Player : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnNicknameChanged))] 
	    public NetworkString<_32> Nickname { get; set; }

        [Networked(OnChanged = nameof(OnCharacterIndexChanged))] 
	    public int CharacterIndex { get; set; }

        private static Player _local;
        public static Player Local => _local;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                _local = this;

                RPC_SetNickame(PlayerPrefs.GetString(Constants.Prefs.Player.Nickname));
            }

            NetworkManager.Instance.SetPlayer(Object.InputAuthority, this);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetNickame(NetworkString<_32> name)
        {
            Nickname = name;
        }

        public static void OnNicknameChanged(Changed<Player> changed)
        {
            NetworkManager.Instance.UpdatePlayer(changed.Behaviour);
        }

        public void SetCharacterIndex(int characterIndex)
        {
            if (HasInputAuthority)
                RPC_SetCharacterIndex(characterIndex);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetCharacterIndex(int characterIndex)
        {
            CharacterIndex = characterIndex;
        }

        public static void OnCharacterIndexChanged(Changed<Player> changed)
        {
            NetworkManager.Instance.UpdatePlayer(changed.Behaviour);
        }
    }
}
