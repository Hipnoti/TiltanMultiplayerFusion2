using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    private NetworkRunner networkRunner;
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    
    public SpawnPoint[] twoPlayerSpawnPoints;
    public SpawnPoint[] sixPlayerSpawnPoints;
    
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
        RPCSetSpawnPoint(info.Source, spawnSpawnIndex);;
    }
    //
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPCSetSpawnPoint([RpcTarget] PlayerRef targetPlayer, int spawnPointIndex)
    {
        Debug.Log("RPCSetSpawnPoint");
        SpawnPoint targetSpawnPoint = sixPlayerSpawnPoints[spawnPointIndex];
        
        targetSpawnPoint.isTaken = true;
        networkRunner.SpawnAsync(playerPrefab, targetSpawnPoint.transform.position,
            targetSpawnPoint.transform.rotation);
    }
}
