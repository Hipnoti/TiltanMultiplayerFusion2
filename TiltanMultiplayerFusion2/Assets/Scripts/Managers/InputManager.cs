using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class InputManager : NetworkBehaviour, INetworkRunnerCallbacks
    {
        private bool pressedFire;

        public override void Spawned()
        {
            base.Spawned();
            Runner.AddCallbacks(this);
        }

        private void Update()
        {
            if(!pressedFire)
                pressedFire = Mouse.current.leftButton.wasPressedThisFrame;
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
            PlayerChracterInputData playerChracterInputData = new PlayerChracterInputData();

            playerChracterInputData.firePressed = pressedFire;
        
            Vector3 movementVector = Vector3.zero;
            Vector3 rotationVector = Vector3.zero;
            if (Keyboard.current.wKey.isPressed) movementVector += Vector3.forward;
            if (Keyboard.current.sKey.isPressed) movementVector += Vector3.back;
            if (Keyboard.current.aKey.isPressed) movementVector += Vector3.left;
            if (Keyboard.current.dKey.isPressed) movementVector += Vector3.right;
            if (Keyboard.current.leftArrowKey.isPressed) rotationVector += Vector3.down;
            if (Keyboard.current.rightArrowKey.isPressed) rotationVector += Vector3.up;
            
            playerChracterInputData.movementVector = movementVector;
            playerChracterInputData.rotationVector = rotationVector;
            
            input.Set(playerChracterInputData);

            pressedFire = false;
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
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}