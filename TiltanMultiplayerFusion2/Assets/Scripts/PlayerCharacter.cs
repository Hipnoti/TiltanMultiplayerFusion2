using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCharacter : NetworkBehaviour
{
    public LookAtCamera lookAtCamera;
    public Image hpBarImage;
    public int MaxHP;
    
    // [Networked][field: SerializeField]
    // public int HP { get; set; }
    [Networked, OnChangedRender (nameof(HPChanged))] [field: SerializeField]
    public int HP { get; set; }

    [ContextMenu("TakeDamageTest")]
    public void TakeDamageTest()
    {
        TakeDamage(10);
    }
    
    public void TakeDamage(int damage)
    {
      //  if(Object.HasStateAuthority)
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



    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.HasStateAuthority)
        {
            Vector3 movementVector = Vector3.zero;
            if(Keyboard.current.wKey.isPressed)
                movementVector += Vector3.forward;
            if(Keyboard.current.sKey.isPressed)
                movementVector += Vector3.back;
            if(Keyboard.current.aKey.isPressed)
                movementVector += Vector3.left;
            if(Keyboard.current.dKey.isPressed)
                movementVector += Vector3.right;
            
            transform.Translate(movementVector * Runner.DeltaTime);
        }
     }
    
}