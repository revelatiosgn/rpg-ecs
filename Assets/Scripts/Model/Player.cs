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
        public void RPC_SetNickame(NetworkString<_32> name)
        {
            Nickname = name;
        }

        public static void OnNicknameChanged(Changed<Player> changed)
        {
            changed.Behaviour.OnNicknameChanged();
        }

        private void OnNicknameChanged()
        {
            NetworkManager.Instance.UpdatePlayer(this);
        }
    }
}
