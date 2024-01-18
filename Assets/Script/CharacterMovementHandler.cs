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

public class CharacterMovementHandler : NetworkBehaviour
{
    [Header("Move")]
    Vector3 moveDirection;
    float moveSpeed = 10f;
    float jumpTime = 0f;
    float jumpCooldown = 0f;
    float jumpForce = 10f;

    [Networked]
    NetworkBool _isJumping { get; set; } = false;
    [Networked]
    int _jumpCount { get; set; } = 0;
    [Networked]
    int _dodgeCount { get; set; }
    public int playerAttackCount { get; set; }
    [Networked]
    NetworkBool canMove { get; set; } = true;

    [Header("Weapon")]
    //public GameObject playerWeaponHandle;
    //public GameObject playerEquipWeapon;
    //public List<GameObject> playerWeaponPrefab;
    //[Networked(OnChanged = nameof(ChangeWeaponNum))]
    //int weaponNum { get; set; }
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
    public LayerMask groundLayer; // 땅인지 확인하기 위한 레이어 마스크
    float groundCheckRad = 0.2f;

    public bool isRespawnRequsted = false;
    //public GameObject _nickName;

    //public int jumpcount2 = 0;
    void Awake()
    {
        characterInputhandler = GetComponent<CharacterInputhandler>();
        hpHandler = GetComponent<HPHandler>();
        networkRigidbody = GetComponent<NetworkRigidbody>();
        rb = GetComponent<Rigidbody>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        networkObject = GetComponent<NetworkObject>();
        weaponHandler = GetComponent<WeaponHandler>();
        cameraAimAngle = GetComponentInChildren<CameraAimAngle>();
        chatSystem = GetComponent<ChatSystem>();
        camera = localCamera.GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        weaponHandler.SetEq();
        //playerJumpCount = 0;
        jumpForce = 10f;
        cameraAimAngle.camera = camera;

        ChangeWeapon(0);

        //if (HasInputAuthority)
        //{
        //    //조작하는 캐릭터 닉네임 끄기
        //    _nickName.SetActive(false);
        //}
    }
    public void PlayFireEffect()
    {
        ;
    }
    public void ChangeWeapon(int _weaponNum)
    {
        weaponHandler.ChangeWeapon(_weaponNum);
    }


    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        if (input != null)
        {
            moveDirection = new Vector3(dir.x, 0f, dir.y);
        }
    }

    //그냥 update 는 로컬로만 움직임 
    //서버에서 움직일려면 fixedUpdateNetwork
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
        if (HasStateAuthority || HasInputAuthority)
        {
            cameraAimAngle.SetAngle();
        }
        if (GetInput(out NetworkInputData networkInputData))
        {
            Chat(networkInputData);

            if (!canMove)
            {
                networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * 0, networkRigidbody.ReadVelocity().y, moveDirection.z * 0));
                return;

            }
            Move(networkInputData);
            Action(networkInputData);


            //아래로 떨어지면 리스폰
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
                canMove = !canMove;
            }
            if (HasInputAuthority)
                chatSystem.Summit();
        }
    }
    private void Move(NetworkInputData networkInputData)
    {
        
        Vector3 tmp = new Vector3(networkInputData.aimFowardVector.x, 0, networkInputData.aimFowardVector.z);
        transform.forward = tmp;

        Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
        moveDirection.Normalize();
        playerStateHandler.SetInputVec(networkInputData.movementInput);
        networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed));

        //networkRigidbody.Rigidbody.velocity = (new Vector3(moveDirection.x * moveSpeed, networkRigidbody.ReadVelocity().y, moveDirection.z * moveSpeed));
    }
    private void Action(NetworkInputData networkInputData)
    {

        //점프 
        if (networkInputData.isJumpButtonPressed && _jumpCount==0)
        {
            if (Object.HasInputAuthority)
                playerStateHandler.isJumpButtonPressed = true;
            if (HasStateAuthority)
                _jumpCount+=1;
            if(_jumpCount!=0)
                networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            return;
        }
        else
            playerStateHandler.isJumpButtonPressed = false;
        //구르기
        if (networkInputData.isDodgeButtonPressed && _dodgeCount < 1)
        {
            if (Object.HasInputAuthority)
            {
                playerStateHandler.isDodgeButtonPressed = true;
            }
            if (HasStateAuthority)
                ++_dodgeCount;
            return;
        }
        else
            playerStateHandler.isDodgeButtonPressed = false;

        if (playerStateHandler.IsGround() && (rb.velocity.y < 0.03 && rb.velocity.y > -0.03))
        {
            //Debug.Log("Ground초기화");
            if (HasStateAuthority)
            {
                _jumpCount = 0;
                _dodgeCount = 0;
            }
        }

        //공격
        if (networkInputData.isFireButtonPressed)
        {
            weaponHandler.Fire(localCamera.transform.position, networkInputData.aimFowardVector, networkInputData.isFireButtonPressed);
            if (Object.HasInputAuthority)
                playerStateHandler.isFireButtonPressed = true;
            else
                playerStateHandler.isFireButtonPressed = false;
            return;
        }
        
    }
    
    //public void Rotation(float _y)
    //{
    //    transform.rotation = Quaternion.Euler(0, _y, 0);
    //}
    /// <summary>
    /// 이상하게 떨어지면 정상적으로 리스폰
    /// </summary>
    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                Respawn();
            }

        }
    }

    /// <summary>
    /// isRespawnRequsted = ture
    /// </summary>

    public void RequestRespawn() => isRespawnRequsted = true;

    /// <summary>
    /// 강제이동 후
    /// hpHandler.OnRespawned();
    /// isRespawnRequsted = false;
    /// </summary>
    void Respawn()
    {
        SetCharacterControllerEnabled(true);
        networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();

        isRespawnRequsted = false;
    }
    /// <summary>
    /// CharacterController bool값으로 키고 끄기 
    /// </summary>
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkRigidbody.enabled = isEnabled;
    }
    public bool IsGround()
    {
        //발밑체크
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
    
}
