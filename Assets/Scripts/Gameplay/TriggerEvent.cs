using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class TriggerEvent : MonoBehaviour
    {
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerExit;

        private void OnTriggerEnter(Collider collider)
        {
            TriggerEnter?.Invoke(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            TriggerExit?.Invoke(collider);
        }
    }
}


