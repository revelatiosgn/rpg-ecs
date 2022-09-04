using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using TMPro;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class NetworkCounter : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _label;

	    [Networked(OnChanged = nameof(OnChangedCounter))]
	    public int Counter { get; set; }

        private float _timer;

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                _timer -= Runner.DeltaTime;
                if (_timer <= 0)
                {
                    Counter++;
                    _timer = 1f;
                }
            }
        }

        private static void OnChangedCounter(Changed<NetworkCounter> changed)
        {
            changed.Behaviour._label.text = changed.Behaviour.Counter.ToString();
        }
    }
}

