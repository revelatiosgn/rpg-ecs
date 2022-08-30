using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.KCC;
using RPGGame.Model;
using TMPro;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class Workbench : NetworkBehaviour
    {
        [SerializeField] private WorkbenchConfig _workbenchConfig;
        public WorkbenchConfig WorkbenchConfig => _workbenchConfig;

        [SerializeField] private TMP_Text _label;

        private void Awake()
        {
            _label.text = _workbenchConfig.Name;
        }
    }
}

