using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Multiplayer;


public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static LobbyManager Instance;
    public const string GAME_SCENE_NAME = "GameScene";

   // [SerializeField] private GameObject readyManagerGeneric;
    [SerializeField] private ReadyManager readyManagerPrefab;
    [SerializeField] NetworkRunner networkRunner;

    [Header("UI References")] [SerializeField]
    private GameObject sessionPanel;

    [SerializeField] private Button sendReadyButton;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private Button endSessionButton;
    [SerializeField] private Button startMatchButton;
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    
    public ReadyManager readyManagerInstance;
    private bool isReadyLocal = false;
    
    private void Start()
    {
        Instance = this;
        networkRunner.AddCallbacks(this);
#if LOBBY_MANAGER_UI
        endSessionButton.interactable = false;
        startMatchButton.interactable = false;
        sendReadyButton.interactable = false;
#endif
    }

    public void StartSession()
    {
        networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "OurGameID",
            OnGameStarted = OnGameStarted
        });

#if LOBBY_MANAGER_UI
        startSessionButton.interactable = false;
        #endif
    }

    private void OnGameStarted(NetworkRunner obj)
    {
        Debug.Log("Game Started");
#if LOBBY_MANAGER_UI
        endSessionButton.interactable = true;
        sendReadyButton.interactable = true;
    //    startMatchButton.interactable = true;
#endif
        if(networkRunner.IsSharedModeMasterClient)
         networkRunner.Spawn(readyManagerPrefab);
        // if (networkRunner.IsSharedModeMasterClient)
        //     networkRunner.Spawn(readyManagerGeneric);
    }

    public void EndSession()
    {
        if (networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
        }
#if LOBBY_MANAGER_UI
        startSessionButton.interactable = true;
        endSessionButton.interactable = false;
        startMatchButton.interactable = false;
        sendReadyButton.interactable = false;
#endif
    }

    public async void JoinLobby()
    {
       StartGameResult result = 
           await networkRunner.JoinSessionLobby(SessionLobby.Custom, "MainLobby");
     
       if (result.Ok)
       {
           Debug.Log("Joined Lobby!");
       }
    }

    [ContextMenu( "Join Lobby Test")]
    public void LeaveLobbyTest()
    {
        networkRunner.JoinSessionLobby(SessionLobby.Shared);
    }

    private void RefreshRoomUI()
    {
#if LOBBY_MANAGER_UI
        if (networkRunner.IsRunning && !networkRunner.IsShutdown)
        { 
            sessionPanel.SetActive(true);
            numberOfPlayersText.text = networkRunner.SessionInfo?.PlayerCount.ToString();
        }
        else
        {
            sessionPanel.SetActive(false);
        }
#endif
    }

    public void StartMatch()
    {
        networkRunner.LoadScene(GAME_SCENE_NAME);
    }
    
    public void SetReady()
    {
        if (!isReadyLocal)
        {
            isReadyLocal = true;
            readyManagerInstance.SetReadyRPC();
            sendReadyButton.interactable = false;
        }
    }

    public void MaxPlayersReady()
    {
        startMatchButton.interactable = true;
    }

    #region RunnerCallBacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        bool isLocalPlayer = networkRunner.LocalPlayer == player;

        Debug.Log($"Player {player.PlayerId} joined, localPlayer: {isLocalPlayer}");

        RefreshRoomUI();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} left");
        RefreshRoomUI();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        RefreshRoomUI();

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
        Debug.Log("Connected to server and lobby successfully!");

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session list updated. Found {sessionList.Count} sessions.");
        foreach (var session in sessionList)
        {
            Debug.Log($"Session Name: {session.Name}, Player Count: {session.PlayerCount}");
        }
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        networkRunner.RemoveCallbacks(this);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    #endregion


  

    // public async void StartSession()
    // {
    //    StartGameResult resTask = await networkRunner.StartGame(new StartGameArgs()
    //    {
    //       GameMode = GameMode.Shared,
    //       SessionName = "OurGameID",
    //       OnGameStarted = OnGameStarted
    //    });
    //
    //    if (resTask.Ok)
    //    {
    //       OnGameStarted(networkRunner);
    //    }
    //    else
    //    {
    //       Debug.LogError($"Game failed to start because {resTask.ErrorMessage}");
    //    }
    // }
    //
    // private void OnGameStarted(NetworkRunner obj)
    // {
    //    Debug.Log("Game Started");;
    // }
}