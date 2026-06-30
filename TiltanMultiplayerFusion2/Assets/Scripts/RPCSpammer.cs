using Fusion;
using UnityEngine;

public class RPCSpammer : NetworkBehaviour
{
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        SomeRPC("INFO",10, 130);
    }

    [Rpc]
    void SomeRPC(string message, int number1, int number2)
    {
        
    }
}
