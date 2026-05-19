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
    
    [Header("Movement")] 
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] float speed = 10f;
    
  //  [Networked, OnChangedRender (nameof(HPChanged))] [field: SerializeField]
    public int HP
    {
        get;
        set;
    }
    

    [ContextMenu("TakeDamageTest")]
    public void TakeDamageTest()
    {
        TakeDamage(10);
    }

    // [Rpc]
    // public void RPCTakeDamage(int damage)
    // {
    //     //We should add here validation! 
    //     //   Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    //     TakeDamage(damage);
    // }
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
    //     if (Object.HasStateAuthority)
    //     {
    //         Vector3 movementVector = Vector3.zero;
    //         Vector3 rotationVector = Vector3.zero;
    //         if (Keyboard.current.wKey.isPressed)
    //             movementVector += Vector3.forward;
    //         if (Keyboard.current.sKey.isPressed)
    //             movementVector += Vector3.back;
    //         if (Keyboard.current.aKey.isPressed)
    //             movementVector += Vector3.left;
    //         if (Keyboard.current.dKey.isPressed)
    //             movementVector += Vector3.right;
    //         if (Keyboard.current.leftArrowKey.isPressed)
    //             rotationVector += Vector3.down;
    //         if (Keyboard.current.rightArrowKey.isPressed)
    //             rotationVector += Vector3.up;
    //         
    //         transform.Rotate(rotationVector * (rotationSpeed * Time.deltaTime));
    //         transform.Translate(movementVector * (speed * Time.deltaTime));
    //     }
    // }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.HasStateAuthority)
        {
            Vector3 movementVector = Vector3.zero;
            Vector3 rotationVector = Vector3.zero;
            if(Keyboard.current.wKey.isPressed)
                movementVector += Vector3.forward;
            if(Keyboard.current.sKey.isPressed)
                movementVector += Vector3.back;
            if(Keyboard.current.aKey.isPressed)
                movementVector += Vector3.left;
            if(Keyboard.current.dKey.isPressed)
                movementVector += Vector3.right;
            if(Keyboard.current.leftArrowKey.isPressed)
                rotationVector += Vector3.down;
            if(Keyboard.current.rightArrowKey.isPressed)
                rotationVector += Vector3.up;
            
            transform.Rotate(rotationVector * (rotationSpeed * Runner.DeltaTime));
            transform.Translate(movementVector * (speed * Runner.DeltaTime));
        }
     }
    
}