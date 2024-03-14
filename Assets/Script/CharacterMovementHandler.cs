using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Animations;
using UnityEngine.VFX;

public class CharacterMovementHandler : NetworkBehaviour
{
    [Header("NickName")]
    public string _nickName;

    [Header("Move")]
    Vector3 moveDirection;
    public Transform characterRoot;
    
    float moveSpeed = 5f;
    float jumpTime = 0f;
    float jumpCooldown = 0f;
    float jumpForce = 10f;
    float maxGravity = -8f;

    [Header("State")]
    PlayerStateHandler playerStateHandler;

    [Header("Component")]
    public NetworkRigidbody networkRigidbody;

    public override void Spawned()
    {
        //Component
        networkRigidbody = GetComponent<NetworkRigidbody>();

        //Script
        playerStateHandler = GetComponent<PlayerStateHandler>();

        ActionVind();
        RotateTowards(1f);
    }

    void ActionVind()
    {
        
        #region Event
        //Move
        CharacterHandler.Move += Move;
        //Jump
        CharacterHandler.Jump += Jump;
        //Respawn
        CharacterHandler.Respawn += Respawn;

        #endregion
    }
    //Respawn
    void Respawn(CharacterHandler _characterHandler)
    {
        SetCharacterControllerEnabled(true);
        networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());
    }
    public void Move(Vector2 _dirVector2)
    {
        if (_dirVector2 == Vector2.zero)
        {
            Vector3 tmp22 = networkRigidbody.Rigidbody.velocity;
            float div = 1.3f;
            networkRigidbody.Rigidbody.velocity = (new Vector3(tmp22.x / div, tmp22.y, tmp22.z / div));
            return;
        }
        else
        {
            if (_dirVector2 != Vector2.zero)
            {
                RotateTowards(_dirVector2.x);
            }
        }
        Vector3 moveDirection = transform.right * _dirVector2.x;
        moveDirection.Normalize();
        
        if (networkRigidbody.Rigidbody.velocity.y < maxGravity)
        {
            networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, maxGravity, moveDirection.z * moveSpeed));
        }
        networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, networkRigidbody.Rigidbody.velocity.y, moveDirection.z * moveSpeed));
    }
    void RotateTowards(float dir)
    {
        Vector3 direction;
        Quaternion targetRotation;
        if (dir > 0)
        {
            direction = Vector3.right;
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else if (dir < 0)
        {
            direction = Vector3.left;
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            return;
        }
        characterRoot.transform.rotation = targetRotation;
    }
    public void Jump(CharacterHandler characterHandler)
    {
        Vector3 tmp = Vector3.zero;
        tmp.x = networkRigidbody.Rigidbody.velocity.x;
        tmp.z = networkRigidbody.Rigidbody.velocity.z;
        networkRigidbody.Rigidbody.velocity = tmp;
        networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void Fire()
    {
        playerStateHandler.GetWeaponHandler().Fire();
    }
    
    //Rule
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkRigidbody.enabled = isEnabled;
    }
   
    //Hit
    public void HitAddForce(Vector3 _attackVec, int _force)
    {
        Vector3 tmp = (_attackVec - transform.position);
        tmp.y = 0;
        tmp = tmp.normalized * _force;
        StartCoroutine(HitAddForce(tmp));
    }

    IEnumerator HitAddForce(Vector3 _attackVec)
    {
        _attackVec = _attackVec / 5;
        for (int i = 1; i < 10; i++)
        {
            float Dis = Vector3.Distance(Vector3.zero, i * _attackVec) / 20;
            networkRigidbody.Rigidbody.AddForce(i * _attackVec + Vector3.up * Dis, ForceMode.Impulse);
            yield return new WaitForFixedUpdate();
        }

    }
    
}
