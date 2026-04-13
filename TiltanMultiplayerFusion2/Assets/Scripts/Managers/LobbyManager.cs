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


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public const string GAME_MODE_KEY = "GameMode";
    public const string GAME_SCENE_NAME = "GameScene";
    
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [FormerlySerializedAs("networkRunner")] [SerializeField] NetworkRunner networkRunnerInstance;

    [Header("UI References")] [SerializeField]
    private GameObject sessionPanel;
    
    [SerializeField] private Button startSessionButton;
    [SerializeField] private TextMeshProUGUI numberOfPlayersText;
    
    private bool isReadyLocal = false;

    private void Start()
    {
        Instance = this;

    }

    public async void StartSession()
    {
#if LOBBY_MANAGER_UI
        startSessionButton.interactable = false;
#endif
        
        //1 - Callback method
        networkRunnerInstance.StartGame(new StartGameArgs()
        {
            
            GameMode = GameMode.Shared,
            SessionName = "OurGameID",
            OnGameStarted = OnGameStarted,
        });
        
        //2 - Async Method
       // StartGameResult startGameResult = await networkRunnerInstance.StartGame(new StartGameArgs()
       //  {
       //      
       //      GameMode = GameMode.Shared,
       //      SessionName = "OurGameID",
       //      OnGameStarted = OnGameStarted,
       //  });
       //
       //  if (startGameResult.Ok == false)
       //  {
       //      Debug.LogError($"Game failed to start because {startGameResult.ErrorMessage}, " +
       //                     $"shutdown reason is {startGameResult.ShutdownReason}");
       //  }
    }

    private void OnGameStarted(NetworkRunner thisNetworkRunner)
    {
        Debug.Log("Game Started");
    }
    

  
}

public enum VersusMode {OneVsOne, TwoVsTwo }