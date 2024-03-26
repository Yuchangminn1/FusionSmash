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
    float jumpForce = 6f;
    float maxGravity = -8f;

    [Networked(OnChanged = nameof(ChangeDir))]
    float myDir { get; set; } = 0f;

    [Header("State")]
    PlayerStateHandler playerStateHandler;

    [Header("Component")]
    NetworkRigidbody networkRigidbody;
    CapsuleCollider capsuleCollider;
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
        capsuleCollider = GetComponent<CapsuleCollider>();
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
        _attackVec.z = 0;
        _attackVec = _attackVec.normalized;
        //Debug.Log("전 HitDir = " + _attackVec.x + "Target AngleY = " + characterRoot.transform.rotation.y + "Power = " + _force);
        if (characterRoot.transform.rotation.y < 0)
        {
            _attackVec.x *= -1;
        }
        //Debug.Log("후 HitDir = " + _attackVec.x + "Target AngleY = " + characterRoot.transform.rotation.y + "Power = " + _force);

        _attackVec = _attackVec * _force;
        StartCoroutine(HitAddForce(_attackVec));
    }

    IEnumerator HitAddForce(Vector3 _attackVec)
    {
        float DivForce = 10f;
        float UpperForce = DivForce;
        _attackVec = _attackVec / DivForce;
        _attackVec.y = _attackVec.x > 0 ? _attackVec.x / UpperForce : -_attackVec.x / UpperForce;
        int maxC =12;
        for (int i = 1; i < maxC; i++)
        {
            networkRigidbody.Rigidbody.AddForce((maxC-i) * _attackVec , ForceMode.Impulse);
            yield return new WaitForFixedUpdate();
        }

    }
    


}
