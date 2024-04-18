using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using Cinemachine;
public class CharacterMovementHandler : NetworkBehaviour
{

    [Header("NickName")]
    public string _nickName;

    [Header("Move")]
    Vector3 moveDirection;
    public Transform characterRoot;


    float moveSpeed = 5f;
    float dampener = 4f;
    //float jumpTime = 0f;
    //float jumpCooldown = 0f;
    float jumpForce = 6f;
    float maxGravity = -20f;


    [Networked(OnChanged = nameof(ChangeDir))]
    float myDir { get; set; } = 0f;

    [Header("State")]
    PlayerStateHandler playerStateHandler;

    [Header("Component")]
    NetworkRigidbody networkRigidbody;
    CapsuleCollider capsuleCollider;

    PlayerActionEvents playerActionEvents;


    float dir;

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


        if (HasStateAuthority || HasInputAuthority)
        {
            networkRigidbody = GetComponent<NetworkRigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            playerStateHandler = GetComponent<PlayerStateHandler>();

            RotateTowards(1);
            myDir = 1;
        }



    }
    private void Start()
    {

        if (HasInputAuthority)
        {
            GameObject CameraCC = GameObject.Find("TraceCamera");
            CameraCC.GetComponent<CMCameraTest>().plyaerTransform = transform;
        }
    }
    


    void RotateTowards(float _dir)
    {
        if (HasStateAuthority || HasInputAuthority)
        {
            if (myDir == 0)
            {
                myDir = -1f;
            }
            if (HasStateAuthority)
            {
                Vector3 targetVelocity = Vector3.zero;
                Vector3 currentVelocity = networkRigidbody.ReadVelocity();
                currentVelocity.y = 0;

                networkRigidbody.Rigidbody.AddForce(targetVelocity - currentVelocity * networkRigidbody.ReadMass() * dampener, ForceMode.Force);
            }


            Vector3 direction;
            Quaternion targetRotation;
            if (_dir > 0)
            {
                direction = Vector3.left;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else if (_dir < 0)
            {
                direction = Vector3.right;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else
            {
                return;
            }
            networkRigidbody.transform.forward = direction;
            playerActionEvents?.TriggerPlyaerTurn(-networkRigidbody.transform.rotation.y);

            //characterRoot.GetComponent<NetworkObject>().transform.rotation = targetRotation;

        }
    }



    //Rule
    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        //if (networkRigidbody != null)
        //    networkRigidbody.enabled = isEnabled;
    }

    //Hit
    public void HitAddForce(Vector3 _attackVec, int _force)
    {
        _attackVec.z = 0;
        _attackVec = _attackVec.normalized;
        Debug.Log("어택 방향 " + _attackVec);

        _attackVec = _attackVec * _force;
        Debug.Log("어택 방향 2" + _attackVec);
        if (HasStateAuthority)
        {
            StartCoroutine(HitAddForce(_attackVec));
        }
    }

    IEnumerator HitAddForce(Vector3 _attackVec)
    {

        if (networkRigidbody)
        {
            float DivForce = 10f;
            //float UpperForce = DivForce*5f;
            float UpperForce = UnityEngine.Random.Range(1.2f, 1.8f);

            _attackVec = _attackVec / DivForce;
            _attackVec.y = _attackVec.x > 0 ? _attackVec.x / UpperForce : -_attackVec.x / UpperForce;
            Debug.Log($"_attackVec.x =  {_attackVec.x} ");
            int maxC = 12;
            for (int i = 1; i < maxC; i++)
            {
                networkRigidbody.Rigidbody.AddForce((maxC - i) * _attackVec * 10f, ForceMode.Force);
                yield return null;
            }
        }
        else
        {
            yield return null;
        }

        yield return null;
    }

    #region Event
    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {
        playerActionEvents = _playerActionEvents;
        //Death 
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;
        //Move 
        _playerActionEvents.OnPlayerMove += OnPlayerMove;
        //Jump 
        _playerActionEvents.OnPlayerJump += OnPlayerJump;
        //Respawn 
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;
        //Init
        _playerActionEvents.OnPlyaerInit += OnPlyaerInit;
        //GameOver
        _playerActionEvents.OnGameOver += OnGameOver;

        _playerActionEvents.OnVictory += OnVictory;

    }

    public void OnGameOver()
    {
        if (HasStateAuthority)
        {
            networkRigidbody.Rigidbody.useGravity = false;
            networkRigidbody.TeleportToPosition(Utils.GameOverPoint());
            //SetCharacterControllerEnabled(false);
        }


        Debug.Log("Movement OnGameOver");
    }
    //Init
    void OnPlyaerInit()
    {
        if (HasStateAuthority)
        {
            //SetCharacterControllerEnabled(true);
            networkRigidbody.Rigidbody.useGravity = true;
            networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());
        }
    }

    //Death
    void OnPlyaerDeath()
    {
        if (HasStateAuthority)
        {
            //SetCharacterControllerEnabled(false);

        }
    }

    //Respawn
    void OnPlyaerRespawn()
    {
        if (HasStateAuthority)
        {
            //SetCharacterControllerEnabled(true);
            networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());
        }

        Debug.Log("Respawn Teleport");
    }
    public void OnPlayerMove(float _dir)
    {
        //Debug.Log("무브 실행중");
        Vector3 currentVelocity = networkRigidbody.ReadVelocity();

        //Debug.Log(currentVelocity);

        if (_dir == 0)
        {

            Vector3 targetVelocity = Vector3.zero;
            currentVelocity.y = 0;

            networkRigidbody.Rigidbody.AddForce(targetVelocity - currentVelocity * networkRigidbody.ReadMass() * dampener, ForceMode.Force);
        }
        else
        {
            if (_dir > 0)
            {
                myDir = -1;
            }
            else if (_dir < 0)
            {
                myDir = 1;
            }
            Vector3 moveDirection = transform.right * -Math.Abs(_dir);
            moveDirection.Normalize();

            //networkRigidbody.Rigidbody.velocity = new Vector3(0f, networkRigidbody.ReadVelocity().y, 0f);
            networkRigidbody.Rigidbody.AddForce((new Vector3(moveDirection.z * moveSpeed, 0f, moveDirection.x * moveSpeed)) / 15f, ForceMode.VelocityChange);

        }


    }
    public void OnPlayerJump()
    {
        Vector3 tmp = Vector3.zero;
        tmp.x = networkRigidbody.Rigidbody.velocity.x;
        tmp.z = networkRigidbody.Rigidbody.velocity.z;
        networkRigidbody.Rigidbody.velocity = tmp;
        networkRigidbody.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }
    public void OnVictory()
    {

        RotateTowards(1f);
        if (HasStateAuthority)
        {
            //SetCharacterControllerEnabled(true);
            networkRigidbody.TeleportToPosition(Vector3.zero);
        }
    }
    #endregion


}
