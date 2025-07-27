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
using UnityEngine.Serialization;


public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static LobbyManager Instance;
    public const string GAME_MODE_KEY = "GameMode";
    public const string GAME_SCENE_NAME = "GameScene";

    [SerializeField] private ReadyManager readyManagerPrefab;
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [FormerlySerializedAs("networkRunner")] [SerializeField] NetworkRunner networkRunnerInstance;

    [Header("UI References")] [SerializeField]
    private GameObject sessionPanel;

    [SerializeField] private TMP_Dropdown versusModeDropdown;
    [SerializeField] private Button joinRandomSessionButton;
    [SerializeField] private Button sendReadyButton;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private Button endSessionButton;
    [SerializeField] private Button startMatchButton;
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    
    [HideInInspector] public ReadyManager readyManagerInstance;
    private bool isReadyLocal = false;

    private void Start()
    {
        Instance = this;
        networkRunnerInstance.AddCallbacks(this);
        networkRunnerInstance.ProvideInput = true;
#if LOBBY_MANAGER_UI
        endSessionButton.interactable = false;
        startMatchButton.interactable = false;
        sendReadyButton.interactable = false;
#endif

        // Populate the versusModeDropdown with the VersusMode enum values
        versusModeDropdown.ClearOptions();
        List<string> versusModeOptions = new List<string>(System.Enum.GetNames(typeof(VersusMode)));
        versusModeDropdown.AddOptions(versusModeOptions);
    }

    public async void StartSessionShared()
    {
#if LOBBY_MANAGER_UI
        startSessionButton.interactable = false;
        joinRandomSessionButton.interactable = false;
#endif
        
       StartGameResult startGameResult = await networkRunnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "OurGameID",
            OnGameStarted = OnGameStarted,
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                {GAME_MODE_KEY, versusModeDropdown.value}
            }
        });

        if (startGameResult.Ok == false)
        {
            Debug.LogError($"Game failed to start because {startGameResult.ErrorMessage}, " +
                           $"shutdown reason is {startGameResult.ShutdownReason}");
        }
    }
    
    public async void StartSessionHost()
    {
#if LOBBY_MANAGER_UI
        startSessionButton.interactable = false;
#endif
        StartGameResult startGameResult = await networkRunnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "OurGameID",
            OnGameStarted = OnGameStarted,
        });

        if (startGameResult.Ok == false)
        {
            Debug.LogError($"Game failed to start because {startGameResult.ErrorMessage}, " +
                           $"shutdown reason is {startGameResult.ShutdownReason}");
        }
    }

    public async void JoinRandomSession()
    {
#if LOBBY_MANAGER_UI
        startSessionButton.interactable = false;
        joinRandomSessionButton.interactable = false;
#endif
        StartGameResult startGameResult = await networkRunnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            OnGameStarted = OnGameStarted,
            EnableClientSessionCreation = false,
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                { GAME_MODE_KEY, versusModeDropdown.value }
            }
        });

        if (startGameResult.Ok)
            Debug.Log("Joined Random Session!");
        else
            Debug.LogError($"Game failed to start because {startGameResult.ErrorMessage}");
    }

    private void OnGameStarted(NetworkRunner thisNetworkRunner)
    {
        Debug.Log("Game Started");
#if LOBBY_MANAGER_UI
        endSessionButton.interactable = true;
        sendReadyButton.interactable = true;
    //    startMatchButton.interactable = true;
#endif
        if(networkRunnerInstance.IsServer)
         networkRunnerInstance.Spawn(readyManagerPrefab);
        // if (networkRunner.IsSharedModeMasterClient)
        //     networkRunner.Spawn(readyManagerGeneric);
        
        foreach (KeyValuePair<string, SessionProperty> sessionProperty in thisNetworkRunner.SessionInfo.Properties)
        { 
            Debug.Log("SessionProperty: " + sessionProperty.Key + " " + sessionProperty.Value.PropertyValue + "");
        }
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
        startMatchButton.interactable = false;
        sendReadyButton.interactable = false;
#endif
    }

    public async void JoinLobby()
    {
       StartGameResult result = 
           await networkRunnerInstance.JoinSessionLobby(SessionLobby.Custom, "MainLobby");
     
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
            joinRandomSessionButton.interactable = true;
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
            readyManagerInstance.SetReadyRPC();
            sendReadyButton.interactable = false;
        }
    }

    public void MaxPlayersReady()
    {
        startMatchButton.interactable = true;
    }

    private void SpawnNewRunner()
    {
        if (networkRunnerInstance != null)
        {
            networkRunnerInstance.RemoveCallbacks(this);
        }
        
        networkRunnerInstance = Instantiate(networkRunnerPrefab);
        networkRunnerInstance.ProvideInput = true;
        networkRunnerInstance.AddCallbacks(this);

        Debug.Log("New NetworkRunner spawned after shutdown");
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
        SpawnNewRunner();
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
            VersusMode sessionVersusMode = (VersusMode)session.Properties[GAME_MODE_KEY].PropertyValue;
            Debug.Log($"Session Versus Mode: {sessionVersusMode}");
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

public enum VersusMode {OneVsOne, TwoVsTwo }