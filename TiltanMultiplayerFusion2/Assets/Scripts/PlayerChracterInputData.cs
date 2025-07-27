using Fusion;
using UnityEngine;


public struct PlayerChracterInputData : INetworkInput
{
    public bool firePressed;
    public Vector3 movementVector;
    public Vector3 rotationVector;
}


