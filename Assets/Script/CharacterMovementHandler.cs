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

public class CharacterMovementHandler : NetworkBehaviour
{
    public CharacterInputhandler characterInputhandler;
    //인풋 전달
    PlayerStateHandler playerStateHandler;

    PlayerInput input;
    Vector3 moveDirection;

    public GameObject playerWeaponHandle;
    public GameObject playerEquipWeapon;
    public GameObject[] playerWeaponPrefab;
    [Networked(OnChanged = nameof(ChangeWeaponNum))]
    int weaponNum { get; set; }

    //InputAction moveAction;

    //[Networked(OnChanged = nameof(OnFireNumChanged))]
    //이 네트워크 클래스가 
    //public NetworkedAttribute()
    //{
    //    OnChangedTargets = OnChangedTargets.All;
    //}
    //로 정보 공유해주는거 
    //프로퍼티 써야지 적용 가능함 
    //int fireNum { get; set; }

    //TextMeshPro textMeshPro;


    public bool isRespawnRequsted = false;

    Camera localCamera;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    HPHandler hpHandler;

    public int jumpcountHas { get; set; }

    float jumpTime = 0f;

    float jumpCooldown = 0.15f;



    //public int playerstate { get; set; }

    public int playerjumpcount { get; set; }

    //public int jumpcount2 = 0;
    void Awake()
    {

        characterInputhandler = GetComponent<CharacterInputhandler>();
        hpHandler = GetComponent<HPHandler>();
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
        playerStateHandler = GetComponent<PlayerStateHandler>();

    }
    // Start is called before the first frame update
    void Start()
    {

        ChangeWeapon(0);
        //textMeshPro = GetComponentInChildren<TextMeshPro>();

    }
    static void ChangeWeaponNum(Changed<CharacterMovementHandler> changed)
    {
        int newWeaponNum = changed.Behaviour.weaponNum;
        changed.LoadOld();
        int oldWeaponNum = changed.Behaviour.weaponNum;
        if (newWeaponNum != oldWeaponNum)
        {
            changed.Behaviour.ChangeWeapon(changed.Behaviour.weaponNum);

        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_WeaponNum(int _weaponNum, RpcInfo info = default)
    {
        if (playerEquipWeapon == null)
            return;
        if (playerEquipWeapon.transform.localPosition != Vector3.zero)
        {
            playerEquipWeapon.transform.localPosition = Vector3.zero;
        }
        weaponNum = _weaponNum;
    }
    void ChangeWeapon(int num)
    {
        if (playerEquipWeapon != null)
            Destroy(playerEquipWeapon);
        weaponNum = num;

        if (Object.HasInputAuthority)
        {
            RPC_WeaponNum(weaponNum);
        }

        if (playerWeaponPrefab.Length > num && playerWeaponPrefab[num] != null)
        {
            playerEquipWeapon = Instantiate(playerWeaponPrefab[num], Vector3.zero, Quaternion.identity);

            playerEquipWeapon.transform.parent = playerWeaponHandle.transform;
            playerEquipWeapon.transform.localPosition = Vector3.zero;

        }

    }
    private void Update()
    {

    }
    //점프 카운트 왜 0이 되는지 이해가 안감 
    //static void jumpcountSet(Changed<CharacterMovementHandler> changed)
    //{
    //    int newC = changed.Behaviour.jumpcountHas;
    //    changed.LoadOld();
    //    int oldC = changed.Behaviour.jumpcountHas;
    //    if (newC != oldC)
    //    {
    //        //Debug.Log($"newC {newC},  oldC{oldC}");
    //        changed.Behaviour.setJumpcount(newC);
    //    }
    //}
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetJumpCount(int _jumpcount, RpcInfo info = default)
    {
        Debug.Log($"jumpcount {_jumpcount} ");
        jumpcountHas = _jumpcount;
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RPC_SetState(int _State, RpcInfo info = default)
    //{
    //    //Debug.Log($"_State {_State} ");
    //    playerstate = _State;
    //}
    //public void setJumpcount(int num)
    //{
    //    jumpcountHas = num;
    //}

    private void FixedUpdate()
    {

    }

    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        if (input != null)
        {
            moveDirection = new Vector3(dir.x, 0f, dir.y);

            Debug.Log("New INput System Value = " + moveDirection);

        }

    }

    //그냥 update 는 로컬로만 움직임 
    //서버에서 움직일려면 fixedUpdateNetwork
    public override void FixedUpdateNetwork()
    {
        //if (Object.HasInputAuthority)
        //{
        //    playerStateHandler.StateChageUpdate();
        //}

        if (HasStateAuthority)
        {

            //상호작용 권한이 있으면
            if (isRespawnRequsted)
            {
                //리스폰이 트루이면 리스폰시켜라
                Respawn();
                return;
            }
            //죽은 상태면 정보 보내지 말고 리턴 
            if (hpHandler.isDead)
                return;
        }



        //Don't update the clients position when they ard dead


        //플레이어 이동 
        //get the input form the network

        //InputHandler 에서 전송한 값을 여기서 받는것으로 보임 

        //즉 네트워크에서 받은 정보를 기반으로 연산을 하는 부분 
        //"현재 시뮬레이션 틱에 대해 유효한 Fusion.INetworkInput을 찾을 수 있다면 true를 반환합니다.
        //반환된 입력 구조체는 Fusion.NetworkObject.InputAuthority에서 시작하며,
        // 유효한 경우 현재 시뮬레이션 틱에 대해 해당 Fusion.PlayerRef가 제공한 입력을 포함합니다
        //GetInput >> NetworkBehavior 에 있는 함수
        if (Object.HasInputAuthority)
        {
            playerStateHandler.StateChageUpdate();
        }

        if (GetInput(out NetworkInputData networkInputData))
        {
            //playerstate = playerStateHandler.state;
            //RPC_SetState(playerstate);
            //Debug.Log($" has 뭐시기{transform.name} = " + networkCharacterControllerPrototypeCustom.Object.HasInputAuthority);w

            //Rotate the transform according to the client aim vector
            transform.forward = networkInputData.aimFowardVector;
            //cancel out rotation on X axis as we don't want our chracter to tilt
            //transform >> 각자의 캐릭터 러너


            //x축 회전을 제거하는 방법 rigidbody 고정이랑 같다고 생각하지뭐

            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;


            //move input값 받아온걸로 계산

            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            playerStateHandler.SetInputVec(networkInputData.movementInput);
            //Debug.Log($"networkInputData.movementInput = {networkInputData.movementInput}");
            if (playerjumpcount != 0)
                Debug.Log($"JumpCount = {playerjumpcount}");

            //Debug.Log("moveDirection = " + moveDirection);
            //Jump 

            //if (networkInputData.isFireButtonPressed || playerstate == 4)
            //{
            //    if (Object.HasInputAuthority)
            //    {
            //        playerStateHandler.isFireButtonPressed = true;
            //    }

            //    if(playerstate < 3 && playerstate >=0)
            //    {
            //        playerStateHandler.nextState = playerStateHandler.attackState;
            //        playerStateHandler.StateChageUpdate();
            //    }
            //    moveDirection.x = 0f;
            //    moveDirection.z = 0f;

            //    networkCharacterControllerPrototypeCustom.Move(moveDirection);
            //    return;

            //}

            networkCharacterControllerPrototypeCustom.Move(moveDirection);
            if (networkInputData.isJumpButtonPressed)
            {
                bool jumpcount = false;

                if (Object.HasInputAuthority)
                {
                    if (jumpTime + jumpCooldown < Time.time)
                    {
                        if (playerStateHandler.state2 < 2)
                        {
                            jumpTime = Time.time;

                            playerStateHandler.isJumpButtonPressed = true;
                            playerStateHandler.StateChageUpdate();


                        }
                        if (playerStateHandler.state == 1)
                        {
                            playerStateHandler.state2 += 1;
                            playerStateHandler.SetState2(playerStateHandler.state2);
                        }

                    }
                }

                if (playerStateHandler.state == 1 && playerStateHandler.state2 <= 2)
                {
                    networkCharacterControllerPrototypeCustom.Jump();
                    Debug.Log($"JUMP실행 ");
                }


            }
            //else if (playerStateHandler.state == 0)
            //{

            //    if (playerStateHandler.IsGround())
            //    {
            //        //Debug.Log("JUmpCountHas 초기화 ");
            //        jumpcountHas = 0;
            //        RPC_SetJumpCount(jumpcountHas);
            //    }

            //}
            //if (Object.HasInputAuthority)
            //    characterInputhandler.isJumpButtonPressed = false;


            //if (networkInputData.isFireButtonPressed)
            //{
            //    ++fireNum;
            //    networkInputData.fireNum = fireNum;
            //}
            CheckFallRespawn();

            //이상하게 떨어지면 정상적으로 리스폰




        }

    }
    //public void RPCJumpCount(int num)
    //{
    //    jumpcountHas = num;
    //    RPC_SetJumpCount(jumpcountHas);
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
                Debug.Log("CheckFallRespawn() 함수로 호출되어 리스폰");

                Respawn();
            }

        }
    }

    /// <summary>
    /// isRespawnRequsted = ture
    /// </summary>

    public void RequestRespawn()
    {
        isRespawnRequsted = true;

    }
    /// <summary>
    /// 강제이동 후
    /// hpHandler.OnRespawned();
    /// isRespawnRequsted = false;
    /// </summary>
    void Respawn()
    {
        SetCharacterControllerEnabled(true);
        networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();

        isRespawnRequsted = false;
    }
    /// <summary>
    /// CharacterController bool값으로 키고 끄기 
    /// </summary>
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }

    //ONchage 쓸떄는 스태틱으로 사용해야함
    //이 함수는 사실 항상 실행되고 있는거고 두 변화값이  다르면 if문을 타고 아래 함수를 실행시키는거임 오우 
    //static void OnFireNumChanged(Changed<CharacterMovementHandler> changed)
    //{

    //    int fireNumCurrent = changed.Behaviour.fireNum;
    //    //Load the old value
    //    changed.LoadOld();
    //    int fireNumOld = changed.Behaviour.fireNum;

    //    if (fireNumCurrent != fireNumOld)
    //        changed.Behaviour.ChangeUItextFireNum(fireNumCurrent);

    //}

    //void ChangeUItextFireNum(int num)
    //{
    //    if (textMeshPro != null)
    //        textMeshPro.text = $"{num}";
    //}

}
