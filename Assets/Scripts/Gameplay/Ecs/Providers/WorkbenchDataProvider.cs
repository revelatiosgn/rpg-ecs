using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class WorkbenchDataProvider : MonoProvider<WorkbenchData>
    {
        [SerializeField] private Workbench _workbench;

        private void Awake()
        {
            value.Workbench = _workbench;
        }
    }
}


