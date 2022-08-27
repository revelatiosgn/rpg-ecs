using System.Collections;
using System.Collections.Generic;
using Fusion;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class PlayerSpawn : SimulationBehaviour, ISpawned
    {
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;
        [SerializeField][Range(0f, 100f)] private float _spawnRadius;

        public void Spawned()
        {
            SpawnCharacters();
        }

        private void SpawnCharacters()
        {
            foreach (Player player in NetworkManager.Instance.Players)
            {
                if (player.Object.HasStateAuthority)
                {
                    Vector3 position = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
                    PlayerCharacter playerCharacter = Runner.Spawn(_playerCharacterPrefab, position, Quaternion.identity, player.Object.InputAuthority);
                }
            }
        }
    }
}


