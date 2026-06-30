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
    
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag(PlayerCharacter.PLAYER_TAG))
        {
            PlayerCharacter player = collider.gameObject.GetComponent<PlayerCharacter>();
            Instantiate(player.hitEffectPrefab, player.transform.position, Quaternion.identity);
            if (HasStateAuthority)
            {
                // if (!player.HasStateAuthority)
                // {
                    player.RPCTakeDamage(10);
                    Runner.Despawn(Object);
               // }
            }
            
        }

        //Add here projetcile hit particle instanatiate
    }
}
