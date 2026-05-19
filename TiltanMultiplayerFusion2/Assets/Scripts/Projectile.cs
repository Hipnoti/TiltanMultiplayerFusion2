using System;
using Fusion;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(Object.HasStateAuthority)
         transform.Translate(Vector3.forward * speed * Runner.DeltaTime);
        
        lifetime -= Runner.DeltaTime;
        if(lifetime <= 0)
            Runner.Despawn(Object);
    }
}
