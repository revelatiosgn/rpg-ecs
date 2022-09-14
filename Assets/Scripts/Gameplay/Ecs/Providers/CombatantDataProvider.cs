using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo.Lite;

namespace RPGGame.Gameplay.Ecs
{
    public sealed class CombatantDataProvider : MonoProvider<CombatantData>
    {
        [SerializeField] private Combatant _combatant;

        private void Awake()
        {
            value.Combatant = _combatant;
        }
    }
}


