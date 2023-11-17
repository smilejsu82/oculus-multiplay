using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Test3Main : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameMode gameMode;
    [SerializeField] private Button btnLogin;
    [SerializeField] private Button btnJoin;
    [SerializeField] private Button btnLoadScene;
    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private Test3Player localPlayer;
    [SerializeField] private GameObject hostVRGo;
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Transform leftHandAnchor;
    [SerializeField] private Transform rightHandAnchor;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (this.gameMode == GameMode.Host)
        {
            this.StartGame(this.gameMode);
            this.btnLoadScene.onClick.AddListener(() =>
            {
                this._runner.SetActiveScene(1);
            });
        }
        else if (this.gameMode == GameMode.Client)
        {
            this.btnJoin.onClick.AddListener(() =>
            {
                this.StartGame(this.gameMode);
            });
        }
        
        EventDispatcher.instance.AddListener("Spawned", (obj) =>
        {
            Test3Player test3Player = obj as Test3Player;
            test3Player.Init(_runner);
            
            //host : true, true
            Debug.LogFormat("[Spawned callback] : {0} -> {1}", test3Player.Id, test3Player.HasInputAuthority);
            
            if (test3Player.HasInputAuthority)
            {
                this.localPlayer = obj as Test3Player;
                
                if(_runner.IsServer) //host
                { 
                    this.localPlayer.SetCenterEyeAnchor(this.centerEyeAnchor);
                    this.localPlayer.SetleftHandAnchor(this.leftHandAnchor);
                    this.localPlayer.SetRightHandAnchor(this.rightHandAnchor);
                }
                else
                {
                    //client (local)
                }
            }
            else
            {
                //clinet (remote)
            }


            // this.btnLogin.onClick.AddListener(() =>
            // {
            //     this.localPlayer.RPC_Configure("홍길동");
            // });
        });
    }

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "1234"
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.LogFormat("OnPlayerJoined: {0}", runner.IsServer);
        if (runner.IsServer)
        {
            // Create a unique position for the player
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
        
        //내 Test3Player를 알수 있는 방법은?
        Debug.LogFormat("_spawnedCharacters : {0}", _spawnedCharacters.Count);

        if (_spawnedCharacters.Count == 2 && runner.IsServer)
        {
            this.hostVRGo.SetActive(true);
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //클라이언트가 서버에 접속 함 
        Debug.Log("OnConnectedToServer");
    }


    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}