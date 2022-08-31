using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Gameplay
{
    [CreateAssetMenu(menuName = "RPG/Characters/CharactersConfig")]
    public class CharactersConfig : ScriptableObject
    {
        [SerializeField] private List<PlayerCharacter> _playerCharacterPrefabs;
        public List<PlayerCharacter> PlayerCharacterPrefabs => _playerCharacterPrefabs;
    }
}
