using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.KCC;
using RPGGame.Gameplay.Ecs;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        [SerializeField] private InputActionReference _switchBehaviourAction;

        [Networked(OnChanged = nameof(OnChangedBehaviourState))]
        public PlayerBehaviourState BehaviourState { get; set; }

        private void Start()
        {
            _switchBehaviourAction.action.Enable();
        }
        
        private void Update()
        {
            if (HasInputAuthority)
            {
                if (_switchBehaviourAction.action.WasReleasedThisFrame())
                {
                    if (BehaviourState == PlayerBehaviourState.Labor)
                        RPC_SwitchBehaviour(PlayerBehaviourState.Combatant);
                    else if (BehaviourState == PlayerBehaviourState.Combatant)
                        RPC_SwitchBehaviour(PlayerBehaviourState.Labor);
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SwitchBehaviour(PlayerBehaviourState behaviourState)
        {
            EcsManager.EventBus.RaiseEvent<OnPlayerSwitchBehaviour>(new OnPlayerSwitchBehaviour { 
                Entity = GetComponent<EntityObject>().Id,
                BehaviourState = behaviourState
            });
        }

        private static void OnChangedBehaviourState(Changed<PlayerBehaviour> changed)
        {
            Debug.Log($"On chaged beh state {changed.Behaviour.BehaviourState.ToString()}");
        }

        public enum PlayerBehaviourState
        {
            Labor,
            Combatant,
            Dead
        }
    }
}

