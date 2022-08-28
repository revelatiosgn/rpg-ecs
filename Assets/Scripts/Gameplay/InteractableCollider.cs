using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class InteractableCollider : MonoBehaviour
    {
        [SerializeField] private Interactable _interactable;
        public Interactable Interactable => _interactable;
    }
}


