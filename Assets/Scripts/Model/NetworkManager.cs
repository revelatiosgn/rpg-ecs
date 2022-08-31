using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame.Model
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner _networkRunner;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private NetworkSceneManagerBase _networkSceneManager;
        [SerializeField] private bool _debugAutoconnect = false;

        private Dictionary<PlayerRef, Player> _players = new Dictionary<PlayerRef, Player>();
        public ICollection<Player> Players => _players.Values;

	    private InputData _inputData;

        public NetworkRunner NetworkRunner => _networkRunner;

        private static NetworkManager _instance;
        public static NetworkManager Instance => _instance;

        public event Action JoinLobbyStarted;
        public event Action<StartGameResult> JoinLobbyCompleted;
        public event Action CreateSessionStarted;
        public event Action<StartGameResult> CreateSessionCompleted;
        public event Action JoinSessionStarted;
        public event Action<StartGameResult> JoinSessionCompleted;
        public event Action<List<SessionInfo>> SessionListUpdated;
        public event Action<PlayerRef> PlayerJoined;
        public event Action<PlayerRef> PlayerLeft;
        public event Action<PlayerRef, Player> PlayerSpawned;
        public event Action<PlayerRef, Player> PlayerDespawned;
        public event Action<Player> PlayerUpdated;
        public event Action LeaveSessionStarted;

        private void Awake()
        {
            if (_instance == null)
            {
			    _instance = this;
                DontDestroyOnLoad(gameObject);

                if (SceneManager.GetActiveScene().buildIndex != 0)
                    SceneManager.LoadScene(0);

                if (_debugAutoconnect)
                {
                    _networkRunner.StartGame(new StartGameArgs
                    {
                        GameMode = GameMode.Host,
                        SceneManager = _networkSceneManager,
                        SessionName = "TestSession"
                    });
                }
                else
                {
                    JoinLobbyTask().Forget();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async UniTask JoinLobbyTask()
        {
            JoinLobbyStarted?.Invoke();
            StartGameResult result = await _networkRunner.JoinSessionLobby(SessionLobby.ClientServer);
            JoinLobbyCompleted?.Invoke(result);
        }

        public async UniTask CreateSessionTask()
        {
            CreateSessionStarted?.Invoke();

            StartGameResult result = await _networkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Host,
                SceneManager = _networkSceneManager,
                PlayerCount = Constants.Server.MaxPlayers
            });

            CreateSessionCompleted?.Invoke(result);
        }

        public async UniTask JoinSessionTask(string sessionName)
        {
            JoinSessionStarted?.Invoke();

            StartGameResult result = await _networkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SceneManager = _networkSceneManager,
                SessionName = sessionName
            });

            JoinSessionCompleted?.Invoke(result);
        }

        public async UniTask LeaveSessionTask()
        {
            LeaveSessionStarted?.Invoke();

            await _networkRunner.Shutdown();

            SceneManager.LoadScene(0);
        }

        public void SetPlayer(PlayerRef playerRef, Player player)
        {
            player.transform.parent = transform;

            _players[playerRef] = player;

            PlayerSpawned?.Invoke(playerRef, player);

            if (_debugAutoconnect)
            {
                StartGame();
            }
        }

        public void UpdatePlayer(Player player)
        {
            PlayerUpdated?.Invoke(player);
        }

        public Player GetPlayer(PlayerRef playerRef)
        {
            if (_players.TryGetValue(playerRef, out Player result))
                return result;

            return null;
        }

        public void StartGame()
        {
            _networkRunner.SetActiveScene("BaseMap");
        }

#region INetworkRunnerCallbacks

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            SessionListUpdated?.Invoke(sessionList);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            if (runner.IsServer)
            {
                Player player = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
            }

            PlayerJoined?.Invoke(playerRef);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
        {
            if (_players.TryGetValue(playerRef, out Player player))
		    {
                runner.Despawn(player.Object);
            }

            _players.Remove(playerRef);

            if (player != null)
                PlayerDespawned?.Invoke(playerRef, player);

            PlayerLeft?.Invoke(playerRef);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            Vector3 moveDirection = default;

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            moveDirection += x * Camera.main.transform.right;
            moveDirection += y * Camera.main.transform.forward;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            _inputData.MoveDirection = moveDirection;

            input.Set(_inputData);

            _inputData.MoveDirection = default;
        }

        public void OnConnectedToServer(NetworkRunner runner) {}
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
        public void OnDisconnectedFromServer(NetworkRunner runner) {}
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}
        public void OnSceneLoadDone(NetworkRunner runner) {}
        public void OnSceneLoadStart(NetworkRunner runner) {}
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}

#endregion

    }
}
