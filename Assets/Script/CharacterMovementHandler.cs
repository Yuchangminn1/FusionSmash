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

    [Header("Script")]
    public CharacterInputhandler characterInputhandler;
    PlayerInput input;
    HPHandler hpHandler;
    CameraAimAngle cameraAimAngle;
    ChatSystem chatSystem;

    [Header("State")]
    PlayerStateHandler playerStateHandler;

    [Header("Component")]
    public NetworkRigidbody networkRigidbody;
    public Rigidbody rb;
    public GameObject localCamera;
    public Camera camera;
    private NetworkObject networkObject;


    [Header("GroundCheck")]
    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer;
    float groundCheckRad = 0.2f;

    //이것들 각자 스크립트로 이동 
    public bool isRespawnRequsted = false;
    public int WeaponCNum = 0;

    public override void Spawned()
    {
        //Input
        characterInputhandler = GetComponent<CharacterInputhandler>();
        //PhotonNetwork
        networkRigidbody = GetComponent<NetworkRigidbody>();
        networkObject = GetComponent<NetworkObject>();
        //chat
        chatSystem = GetComponent<ChatSystem>();
        //Component
        rb = GetComponent<Rigidbody>();
        camera = localCamera.GetComponent<Camera>();

        //Script
        hpHandler = GetComponent<HPHandler>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        cameraAimAngle = GetComponentInChildren<CameraAimAngle>();

        //Camera
        cameraAimAngle.camera = camera;

        

        RotateTowards(1f);
    }
    
    

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (isRespawnRequsted)
            {
                Respawn();
                return;
            }
            if (hpHandler.isDead)
                return;
        }
        cameraAimAngle.SetAngle();
        
        if (GetInput(out NetworkInputData networkInputData))
        {
            ShowBoard(networkInputData);
            Chat(networkInputData);

            Move(networkInputData);
            Action(networkInputData);

            CheckFallRespawn();
            playerStateHandler.StateChageUpdate();
        }
    }

    
    private void Move(NetworkInputData networkInputData)
    {

        Vector3 tmp = new Vector3(networkInputData.aimFowardVector.x, 0, networkInputData.aimFowardVector.z);
        transform.forward = tmp;

        Vector3 moveDirection = transform.right * networkInputData.movementInput.x;
        moveDirection.Normalize();

        if (GetStopMove())
        {
            Vector3 tmp22 = networkRigidbody.Rigidbody.velocity;
            float div = 1.3f;
            networkRigidbody.Rigidbody.velocity = (new Vector3(tmp22.x / div, tmp22.y, tmp22.z / div));

        }
        if (!GetCanMove())
        {
            return;
        }
        if (networkInputData.movementInput != Vector2.zero)
        {
            RotateTowards(networkInputData.movementInput.x);
        }
        playerStateHandler.SetInputVec(networkInputData.movementInput);
        if (rb.velocity.y < maxGravity)
        {
            networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, maxGravity, moveDirection.z * moveSpeed));
        }
        networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed));
    }
    private void Action(NetworkInputData networkInputData)
    {
        //if (!HasStateAuthority) return;

        if (playerStateHandler.IsGround() && (rb.velocity.y < 0.03 && rb.velocity.y > -0.03) && !playerStateHandler.Isvisi())
        {
            //Reset Counter
            playerStateHandler.ResetJumpCount();
            //_dodgeCount = 0;
        }
        //Jump
        if (networkInputData.isJumpButtonPressed)
        {
            if (playerStateHandler.JumpAble())
            {
                Jump();
                return;
            }
        }
        //Fire
        if (networkInputData.isFireButtonPressed)
        {

            if (!playerStateHandler.AbleFire())
            {
                playerStateHandler.isFireButtonPressed = false;
                return;
            }
            else
            {
                playerStateHandler.isFireButtonPressed = true;
                playerStateHandler.AddAttackCount();
                playerStateHandler.GetWeaponHandler().SetFire(localCamera.transform.position, networkInputData.aimFowardVector, networkInputData.isFireButtonPressed);
                return;
            }
        }
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
    private void Jump()
    {
        Vector3 tmp = Vector3.zero;
        tmp.x = rb.velocity.x;
        tmp.z = rb.velocity.z;
        rb.velocity = tmp;
        networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void Fire()
    {
        playerStateHandler.GetWeaponHandler().Fire();
    }
    //Respawn
    protected virtual void Respawn()
    {
        SetCharacterControllerEnabled(true);
        networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());
        playerStateHandler.GetWeaponHandler()._equipWeapon.OnRespawn();

        hpHandler.OnRespawned();
        isRespawnRequsted = false;
    }
    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                hpHandler.KillSelf();
            }
        }
    }
    public void RequestRespawn() => isRespawnRequsted = true;

    //Rule
    public bool IsGround()
    {
        Vector3 tmp = transform.position;

        if (Physics.Raycast(transform.position + Vector3.up * GroundCheckDis / 2f, Vector3.down, GroundCheckDis, groundLayer))
        {
            return true;
        }
        else
        {
            bool _l = Physics.Raycast(transform.position + Vector3.left * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            bool _r = Physics.Raycast(transform.position + Vector3.right * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            bool _f = Physics.Raycast(transform.position + Vector3.forward * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            bool _b = Physics.Raycast(transform.position + Vector3.back * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            if (_l && _r && _f && _b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    bool GetCanMove()
    {
        return playerStateHandler.canMove;
    }
    bool GetStopMove()
    {
        return playerStateHandler.stopMove;
    }
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
            rb.AddForce(i * _attackVec + Vector3.up * Dis, ForceMode.Impulse);
            yield return new WaitForFixedUpdate();
        }

    }
    //UI
    void ShowBoard(NetworkInputData networkInputData)
    {
        if (networkInputData.isShowBoardButtonPressed)
        {
            //if(HasStateAuthority)
            //    _showBoard = !_showBoard;
        }
    }
    private void Chat(NetworkInputData networkInputData)
    {
        if (networkInputData.isChatButtonPressed)
        {
            if (HasStateAuthority)
            {
                playerStateHandler.SetCanMove(chatSystem.ischating);
                chatSystem.IsChatChange();
            }
        }
    }
}
