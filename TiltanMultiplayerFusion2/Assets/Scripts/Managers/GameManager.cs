using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public const string LOBBY_SCENE_NAME = "LobbyScene";

    public static GameManager Instance;
    public Camera mainCamera;
    public InputManager inputManagerPrefab;
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    
    private NetworkRunner networkRunner;

    public SpawnPoint[] twoPlayerSpawnPoints;
    public SpawnPoint[] sixPlayerSpawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        networkRunner = NetworkRunner.GetRunnerForScene(SceneManager.GetActiveScene());
        //Option 1
        //      networkRunner.SpawnAsync(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        // Option 2
        // SpawnPoint targetSpawnPoint;
        //
        // if (networkRunner.IsSharedModeMasterClient)
        // {
        //     targetSpawnPoint = twoPlayerSpawnPoints[0];
        // }
        // else
        // {
        //     targetSpawnPoint = twoPlayerSpawnPoints[1];
        // }
        //
        // networkRunner.SpawnAsync(playerPrefab, targetSpawnPoint.transform.position,
        //     targetSpawnPoint.transform.rotation);

        //Option 3
        // SpawnPoint targetSpawnPoint;
        // do
        // {
        //     targetSpawnPoint = sixPlayerSpawnPoints[Random.Range(0, sixPlayerSpawnPoints.Length)];
        // } while (targetSpawnPoint.isTaken);
        //
        // targetSpawnPoint.isTaken = true;
        // networkRunner.SpawnAsync(playerPrefab, targetSpawnPoint.transform.position,
        //     targetSpawnPoint.transform.rotation);
    }

    public override void Spawned()
    {
        base.Spawned();
        RPCRequestSpawnPoint();
    }

    public void LeaveGame()
    {
        if (networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
        }

        SceneManager.LoadScene(LOBBY_SCENE_NAME);
    }

    //
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPCRequestSpawnPoint(RpcInfo info = default)
    {
        int spawnSpawnIndex = 0;
        SpawnPoint targetSpawnPoint;
        do
        {
            spawnSpawnIndex = Random.Range(0, sixPlayerSpawnPoints.Length);
            targetSpawnPoint = sixPlayerSpawnPoints[spawnSpawnIndex];
        } while (targetSpawnPoint.isTaken);

        targetSpawnPoint.isTaken = true;
        
        //WE GOT THE POWER!
        if(networkRunner.GameMode == GameMode.Shared)
          RPCSetSpawnPoint(info.Source, spawnSpawnIndex);
        else if(networkRunner.IsServer)
        {
            NetworkSpawnOp op = networkRunner.SpawnAsync(playerPrefab, targetSpawnPoint.transform.position,
                targetSpawnPoint.transform.rotation, info.Source);
        }
    }

    //
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private async void RPCSetSpawnPoint([RpcTarget] PlayerRef targetPlayer, int spawnPointIndex)
    {
        Debug.Log("RPCSetSpawnPoint");
        SpawnPoint targetSpawnPoint = sixPlayerSpawnPoints[spawnPointIndex];

        targetSpawnPoint.isTaken = true;
        NetworkSpawnOp op = networkRunner.SpawnAsync(playerPrefab, targetSpawnPoint.transform.position,
        networkRunner.Spawn(inputManagerPrefab);

        await op;
        //Only HOST/Shared mode client can do this!!!
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
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

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
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
        int ahui = 10;
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
