using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCharacter : NetworkBehaviour
{
    public const string PLAYER_TAG = "Player";
    public LookAtCamera lookAtCamera;
    public Image hpBarImage;
    public int MaxHP;
    
    NetworkMecanimAnimator animator;

    public GameObject hitEffectPrefab;
    
    [Header("Movement")] 
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] float speed = 10f;
    
    [Header("Projectile")]
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    
    [Networked, OnChangedRender (nameof(HPChanged))] [field: SerializeField]
    public int HP
    {
        get;
        set;
    }

    private int obsedHP = 0;
    private bool pressedFire = false;

    [ContextMenu("TakeDamageTest")]
    public void TakeDamageTest()
    {
        TakeDamage(10);
    }

    [Rpc]
    public void RPCTakeDamage(int damage)
    {
        //We should add here validation! 
        //   Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        TakeDamage(damage);
    }
    public void TakeDamage(int damage)
    {
        if(Object.HasStateAuthority)
            HP -= damage;
    }

    private void HPChanged()
    {
        HP = Mathf.Clamp(HP, 0, MaxHP);
        hpBarImage.fillAmount = HP / (float)MaxHP;
        if (HP <= 0)
        {
            Debug.Log($"{Object.StateAuthority.PlayerId} has died!");
            if (HasStateAuthority)
                Runner.Despawn(Object);
        }

    }

    // private void Update()
    // {
    //     if(!pressedFire)
    //         pressedFire = Mouse.current.leftButton.wasPressedThisFrame;
    // }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        //Without client prediction
        if (Object.HasStateAuthority)
        {
            if (GetInput(out PlayerChracterInputData data))
            {
                Vector3 movementVector = data.movementVector;
                Vector3 rotationVector = data.rotationVector;

                transform.Rotate(rotationVector *
                                 (rotationSpeed * Runner.DeltaTime));
                transform.Translate(movementVector *
                                    (speed * Runner.DeltaTime));


                if (data.firePressed)
                    SpawnProjectile();
            }
        }


        // if (GetInput(out PlayerChracterInputData data))
        // {
        //     Vector3 movementVector = data.movementVector;
        //     Vector3 rotationVector = data.rotationVector;
        //
        //     transform.Rotate(rotationVector *
        //                      (rotationSpeed * Runner.DeltaTime));
        //     transform.Translate(movementVector *
        //                         (speed * Runner.DeltaTime));
        //
        //     if (Object.HasStateAuthority)
        //     {
        //         if (data.firePressed)
        //             SpawnProjectile();
        //     }
        // }
    }

    void SpawnProjectile()
    {
        if (Object.HasStateAuthority)
        {
            Projectile projectile = 
                Runner.Spawn(projectilePrefab,
                    projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        }
    }
    
    
    [ContextMenu("ReleaseStateAuthorirty")]
    public void ReleaseStateAuthority()
    {
        if (Object.HasStateAuthority)
        {
            Object.ReleaseStateAuthority();
            Debug.Log("Released State Authority");
        }
    }
    
    [ContextMenu("RequestStateAuthority")]
    public void RequestStateAuthority()
    {
        if (!Object.HasStateAuthority)
        {
            Object.RequestStateAuthority();
            Debug.Log("Requested State Authority");
        }
    }
}