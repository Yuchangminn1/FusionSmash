using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using System.Collections; 
using System;
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
    float jumpForce = 8f;
    float maxGravity = -8f;

    [Networked(OnChanged = nameof(ChangeDir))]
    float myDir { get; set; } = 0f;

    [Header("State")]
    PlayerStateHandler playerStateHandler;

    [Header("Component")]
    public NetworkRigidbody networkRigidbody;
    
    static void ChangeDir(Changed<CharacterMovementHandler> changed)
    {
        float newS = changed.Behaviour.myDir;
        changed.LoadOld();
        float oldS = changed.Behaviour.myDir;
        if (newS != oldS)
        {
            changed.Behaviour.RotateTowards(newS);
        }
    }
    public override void Spawned()
    {
        //Component
        networkRigidbody = GetComponent<NetworkRigidbody>();

        //Script
        playerStateHandler = GetComponent<PlayerStateHandler>();

        RotateTowards(1);
    }
    #region Event
    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        if (_playerActionEvents == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }
        //Death 
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;
        //Move 
        _playerActionEvents.OnPlayerMove += OnPlayerMove;
        Debug.Log("등록");
        //Jump 
        _playerActionEvents.OnPlayerJump += OnPlayerJump;
        //Respawn 
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;

    }
    //Death
    void OnPlyaerDeath()
    {
        SetCharacterControllerEnabled(false);
    }

    //Respawn
    void OnPlyaerRespawn()
    {
        SetCharacterControllerEnabled(true);
        networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());
    }
    public void OnPlayerMove(float _dir)
    {
        //Debug.Log("무브 실행중");
        if (_dir == 0)
        {
            Vector3 tmp22 = networkRigidbody.Rigidbody.velocity;
            float div = 1.3f;
            networkRigidbody.Rigidbody.velocity = (new Vector3(tmp22.x / div, tmp22.y, tmp22.z / div));
            return;
        }
        else
        {
            myDir = _dir;
        }
        Vector3 moveDirection = transform.right * _dir;
        moveDirection.Normalize();

        if (networkRigidbody.Rigidbody.velocity.y < maxGravity)
        {
            networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, maxGravity, moveDirection.z * moveSpeed));
        }

        networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, networkRigidbody.Rigidbody.velocity.y, moveDirection.z * moveSpeed));

    }
    public void OnPlayerJump()
    {
        Vector3 tmp = Vector3.zero;
        tmp.x = networkRigidbody.Rigidbody.velocity.x;
        tmp.z = networkRigidbody.Rigidbody.velocity.z;
        networkRigidbody.Rigidbody.velocity = tmp;
        networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    #endregion

    void RotateTowards(float _dir)
    {
        
        Vector3 direction;
        Quaternion targetRotation;
        if (_dir > 0)
        {
            direction = Vector3.right;
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else if (_dir < 0)
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
    


    //Rule
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkRigidbody.enabled = isEnabled;
    }

    //Hit
    public void HitAddForce(Vector3 _attackVec, int _force)
    {
        Vector3 tmp = (_attackVec - transform.position);
        tmp.z = 0;
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
