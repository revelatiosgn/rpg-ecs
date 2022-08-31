using System.Collections;
using System.Collections.Generic;
using Fusion;
using RPGGame.Model;
using UnityEngine;

namespace RPGGame.Gameplay
{
    public class PlayerSpawn : SimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;
        [SerializeField][Range(0f, 100f)] private float _spawnRadius;

        public void Spawned()
        {
            NetworkManager.Instance.PlayerSpawned += PlayerSpawned;

            SpawnCharacters();
        }

        public void Despawned(NetworkRunner runner, bool hasState)
        {
            NetworkManager.Instance.PlayerSpawned -= PlayerSpawned;
        }

        private void SpawnCharacters()
        {
            foreach (Player player in NetworkManager.Instance.Players)
            {
                if (player.Object.HasStateAuthority)
                {
                    SpawnCharacter(player.Object.InputAuthority);
                }
            }
        }

        private void SpawnCharacter(PlayerRef inputAuthority)
        {
            Vector3 position = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
            PlayerCharacter playerCharacter = Runner.Spawn(_playerCharacterPrefab, position, Quaternion.identity, inputAuthority);
        }

        private void PlayerSpawned(PlayerRef playerRef, Player player)
        {
            Debug.Log($"player spawned: {player.Nickname}");
            SpawnCharacter(playerRef);
        }
    }
}


