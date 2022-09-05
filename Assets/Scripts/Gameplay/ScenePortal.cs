using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using TMPro;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class ScenePortal : NetworkBehaviour
    {
        [SerializeField] private string _targetPortal;
        public string TargetPortal => _targetPortal;

        [SerializeField] private TMP_Text _label;

        private static Dictionary<string, ScenePortal> _portals = new Dictionary<string, ScenePortal>();

        private void Awake()
        {
            _portals.Add(gameObject.name, this);
            _label.text = _targetPortal;
        }

        private void OnDestroy()
        {
            _portals.Remove(gameObject.name);
        }

        public static ScenePortal GetPortal(string portalName)
        {
            return _portals[portalName];
        }
    }
}

