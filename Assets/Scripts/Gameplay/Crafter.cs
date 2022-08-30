using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Crafter : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnWorkbenchChanged))]
        public NetworkString<_32> WorkbenchId { get; set; }

        private static void OnWorkbenchChanged(Changed<Crafter> changed)
        {
            NetworkString<_32> id = changed.Behaviour.WorkbenchId;
            if (id.Value != "0")
                Debug.Log($"OnWorkbenchStarted {id.Value}");
            else
                Debug.Log($"OnWorkbenchEnded {id.Value}");
        }
    }
}

