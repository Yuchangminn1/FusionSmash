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
    [Header("Move")]
    Vector3 moveDirection;
    [Networked]
    public NetworkBool canMove { get; set; } = true;
    float moveSpeed = 5f;
    float jumpTime = 0f;
    float jumpCooldown = 0f;
    float jumpForce = 10f;
    float maxGravity = -8f;
    [Header("NickName")]
    public string _nickName;

    [Networked]
    int _jumpGravityCount { get; set; } = 0;
    [Networked]
    int _dodgeCount { get; set; }
    public int playerAttackCount { get; set; }


    [Header("Weapon ")]
    WeaponHandler weaponHandler;



    [Header("Script")]
    public CharacterInputhandler characterInputhandler;
    PlayerInput input;
    HPHandler hpHandler;

    CameraAimAngle cameraAimAngle;

    ChatSystem chatSystem;

    [Header("State")]
    PlayerStateHandler playerStateHandler;
    [Networked]
    NetworkBool isvisi { get; set; } = false;

    [Header("Component")]
    public NetworkRigidbody networkRigidbody;
    public Rigidbody rb;
    public GameObject localCamera;
    public Camera camera;
    private NetworkObject networkObject;


    [Header("Camera")]
    public float rotationSpeed = 15.0f;
    public float viewUpDownRotationSpeed = 50.0f;

    [Header("GroundCheck")]
    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer;
    float groundCheckRad = 0.2f;

    public bool isRespawnRequsted = false;
    //public GameObject _nickName;

    //public int jumpcount2 = 0;
    void Awake()
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
        weaponHandler = GetComponent<WeaponHandler>();
        cameraAimAngle = GetComponentInChildren<CameraAimAngle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        weaponHandler.SetEq();
        cameraAimAngle.camera = camera;
        ChangeWeapon(0);
    }
    public void PlayFireEffect()
    {
        ;
    }
    public void ChangeWeapon(int _weaponNum)
    {
        weaponHandler.ChangeWeapon(_weaponNum);
    }

    void ShowBoard(NetworkInputData networkInputData)
    {
        if (networkInputData.isShowBoardButtonPressed)
        {
            //if(HasStateAuthority)
            //    _showBoard = !_showBoard;
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        if (input != null)
        {
            moveDirection = new Vector3(dir.x, 0f, dir.y);
        }
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
        //if (HasInputAuthority)
        //{
        //    cameraAimAngle.SetAngle();
        //}
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

    private void Chat(NetworkInputData networkInputData)
    {
        if (networkInputData.isChatButtonPressed)
        {

            if (HasStateAuthority)
            {
                SetCanMove(!chatSystem.ischating);
                chatSystem.IsChatChange();
            }
        }


    }
    private void Move(NetworkInputData networkInputData)
    {
        if (HasStateAuthority)
        {
            if (playerStateHandler.state == 2)
            {
                if (networkRigidbody.ReadVelocity().y < 0.05f && networkRigidbody.ReadVelocity().y > -0.05f)
                {
                    _jumpGravityCount++;
                }
            }
            else
            {
                _jumpGravityCount = 0;
            }
        }

        Vector3 tmp = new Vector3(networkInputData.aimFowardVector.x, 0, networkInputData.aimFowardVector.z);
        transform.forward = tmp;

        Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
        moveDirection.Normalize();
        if (!canMove)
        {
            moveDirection = Vector3.zero;
        }
        playerStateHandler.SetInputVec(networkInputData.movementInput);
        if (_jumpGravityCount > 10)
        {
            networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, maxGravity, moveDirection.z * moveSpeed));
        }
        else
        {
            if (rb.velocity.y < maxGravity)
            {
                networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, maxGravity, moveDirection.z * moveSpeed));
            }
            networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed));
        }
    }
    private void Action(NetworkInputData networkInputData)
    {
        //if (!HasStateAuthority) return;

        if (playerStateHandler.IsGround() && (rb.velocity.y < 0.03 && rb.velocity.y > -0.03)&& !playerStateHandler.Isvisi())
        {
            //Reset Counter
            playerStateHandler.ResetJumpCount();
            _dodgeCount = 0;
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

            if (!weaponHandler.AbleFire() || !playerStateHandler.AbleFire()) 
            {

                playerStateHandler.isFireButtonPressed = false;
                return;
            }
            else 
            {
                weaponHandler.SetFire(localCamera.transform.position, networkInputData.aimFowardVector, networkInputData.isFireButtonPressed);
                //Debug.Log(localCamera.transform.position + networkInputData.aimFowardVector);
                playerStateHandler.isFireButtonPressed = true;
                return;
            }
        }
    }

    public void Fire()
    {
        weaponHandler.Fire();
    }

    private void Jump()
    {
        Vector3 tmp = Vector3.zero;
        tmp.x = rb.velocity.x;
        tmp.z = rb.velocity.z;
        rb.velocity = tmp;
        networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    /// <summary>
    /// isRespawnRequsted = ture
    /// </summary>

    public void RequestRespawn() => isRespawnRequsted = true;

    /// <summary>
    /// �����̵� ��
    /// hpHandler.OnRespawned();
    /// isRespawnRequsted = false;
    /// </summary>
    void Respawn()
    {
        SetCharacterControllerEnabled(true);
        networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();
        weaponHandler._equipWeapon.OnRespawn();
        isRespawnRequsted = false;
    }
    /// <summary>
    /// CharacterController bool������ Ű�� ���?
    /// </summary>
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkRigidbody.enabled = isEnabled;
    }
    public bool IsGround()
    {
        //�߹�üũ
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

    public void SetCanMove(bool _tf)
    {

        canMove = _tf;
    }

    public void HitAddForce(Vector3 _attackVec,int _force)
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
            float Dis = Vector3.Distance(Vector3.zero, i * _attackVec)/20;
            rb.AddForce(i*_attackVec+Vector3.up* Dis,ForceMode.Impulse);
            yield return new WaitForFixedUpdate();
        }
        
    }

}
