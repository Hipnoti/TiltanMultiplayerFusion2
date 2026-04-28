using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Multiplayer;
using UnityEngine.Serialization;


public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static LobbyManager Instance;
    public const string GAME_MODE_KEY = "GameMode";
    public const string GAME_SCENE_NAME = "GameScene";
    
    [SerializeField] NetworkRunner networkRunnerInstance;

    [Header("UI References")] [SerializeField]
    private GameObject sessionPanel;
    
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TMP_InputField lobbyNameField;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private Button endSessionButton;
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    
    private bool isReadyLocal = false;

    private void Start()
    {
        Instance = this;
        endSessionButton.interactable = false;
        networkRunnerInstance.AddCallbacks(this);
    }

    public async void StartSession()
    {
        startSessionButton.interactable = false;
        joinLobbyButton.interactable = false;
       StartGameResult startGameResult = await networkRunnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "OurGameIDD",
            OnGameStarted = OnGameStarted
        });
       
        if (!startGameResult.Ok)
        {
            Debug.LogError($"Game failed to start because {startGameResult.ErrorMessage}, " +
                           $"shutdown reason is {startGameResult.ShutdownReason}");
        }
    }

    private void OnGameStarted(NetworkRunner thisNetworkRunner)
    {
        Debug.Log("Game Started");
        endSessionButton.interactable = true;
        
        // foreach (KeyValuePair<string, SessionProperty> sessionProperty in thisNetworkRunner.SessionInfo.Properties)
        // { 
        //     Debug.Log("SessionProperty: " + sessionProperty.Key + " " + sessionProperty.Value.PropertyValue + "");
        // }
    }

    public void EndSession()
    {
        if (networkRunnerInstance.IsRunning)
        {
            networkRunnerInstance.Shutdown();
        }
#if LOBBY_MANAGER_UI
        startSessionButton.interactable = true;
        endSessionButton.interactable = false;
#endif
    }

    public async void JoinLobby()
    {
        joinLobbyButton.interactable = false;
       StartGameResult result = 
           await networkRunnerInstance.JoinSessionLobby(SessionLobby.Custom, lobbyNameField.text);
     
       if (result.Ok)
       {
           Debug.Log("Joined Lobby!");
       }
    }

    [ContextMenu( "Join Lobby Test")]
    public void LeaveLobbyTest()
    {
        networkRunnerInstance.JoinSessionLobby(SessionLobby.Shared);
    }

    private void RefreshRoomUI()
    {
#if LOBBY_MANAGER_UI
        if (networkRunnerInstance.IsRunning && !networkRunnerInstance.IsShutdown)
        { 
            sessionPanel.SetActive(true);
            numberOfPlayersText.text = networkRunnerInstance.SessionInfo?.PlayerCount.ToString();
        }
        else
        {
            sessionPanel.SetActive(false);
            startSessionButton.interactable = true;
        }
#endif
    }

    public void StartMatch()
    {
      //  networkRunnerInstance.SessionInfo.IsVisible = false;
      //  networkRunnerInstance.SessionInfo.IsOpen = false;
        networkRunnerInstance.LoadScene(GAME_SCENE_NAME);
    }
    
    public void SetReady()
    {
        if (!isReadyLocal)
        {
            isReadyLocal = true;
        }
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
        bool isLocalPlayer = networkRunnerInstance.LocalPlayer == player;

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
        Debug.Log("ShutDown call because " + shutdownReason);
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
        networkRunnerInstance.RemoveCallbacks(this);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    #endregion
    
}

public enum VersusMode {OneVsOne, TwoVsTwo }