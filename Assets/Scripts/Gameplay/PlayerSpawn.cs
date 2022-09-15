using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using RPGGame.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Gameplay
{
    public class PlayerSpawn : SimulationBehaviour, ISpawned, IDespawned
    {
        [SerializeField] private CharactersConfig _charactersConfig;
        [SerializeField][Range(0f, 100f)] private float _spawnRadius;

        private static Dictionary<PlayerRef, PlayerCharacter> _playerCharacters = new Dictionary<PlayerRef, PlayerCharacter>();

        public void Spawned()
        {
            NetworkManager.Instance.PlayerSpawned += PlayerSpawned;
            NetworkManager.Instance.PlayerLeft += PlayerLeft;

            if (HasStateAuthority)
            {
                // TODO: get from settings
                NetworkManager.Instance.AdditiveSceneLoader.LoadScene("Map_0", () => SpawnCharacters());
                NetworkManager.Instance.AdditiveSceneLoader.LoadScene("Map_1");
                NetworkManager.Instance.AdditiveSceneLoader.LoadScene("SmallHouse");
            }
        }

        public void Despawned(NetworkRunner runner, bool hasState)
        {
            NetworkManager.Instance.PlayerSpawned -= PlayerSpawned;
            NetworkManager.Instance.PlayerLeft -= PlayerLeft;
        }

        private void SpawnCharacters()
        {
            foreach (Player player in NetworkManager.Instance.Players)
            {
                if (HasStateAuthority)
                {
                    SpawnCharacter(player.Object.InputAuthority, player);
                }
            }
        }

        private void SpawnCharacter(PlayerRef inputAuthority, Player player)
        {
            Vector3 position = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0f, Random.Range(-_spawnRadius, _spawnRadius));
            PlayerCharacter playerCharacter = Runner.Spawn(_charactersConfig.PlayerCharacterPrefabs[player.CharacterIndex], position, Quaternion.identity, inputAuthority);
            _playerCharacters[inputAuthority] = playerCharacter;

            // playerCharacter.SceneIndex = SceneManager.GetSceneByName("Map_0").buildIndex;
        }

        private void PlayerSpawned(PlayerRef playerRef, Player player)
        {
            if (HasStateAuthority)
            {
                Debug.Log($"player spawn: {player.Nickname}");
                SpawnCharacter(playerRef, player);
            }
        }

        private void PlayerLeft(PlayerRef playerRef)
        {
            if (HasStateAuthority)
            {
                Debug.Log($"player despawn");
                if (_playerCharacters.TryGetValue(playerRef, out PlayerCharacter playerCharacter))
                    Runner.Despawn(playerCharacter.Object);
            }
        }
    }
}


