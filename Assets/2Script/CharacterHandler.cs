using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Video.VideoPlayer;


public class CharacterHandler : NetworkBehaviour, IPlayerActionListener
{

    //Handler


    PlayerInfo playerInfo;
    PlayerActionEvents eventHandler;
    CharacterInputhandler inputHandler;
    CharacterMovementHandler movementHandler;
    PlayerStateHandler playerStateHandler;
    WeaponHandler weaponHandler;
    HPHandler hpHandler;
    ChatSystem chatSystem;
    PlayerCharacterUIHandler characterUIHandler;
    //Component
    //public GameObject virtualCamera;
    void Start()
    {


    }
    private void FixedUpdate()
    {
        if (eventHandler)
            eventHandler.TrigerFixedUpdate();
    }
    public override void Spawned()
    {


        //Input
        playerInfo = GetComponent<PlayerInfo>();
        eventHandler = GetComponent<PlayerActionEvents>();
        inputHandler = GetComponent<CharacterInputhandler>();
        movementHandler = GetComponent<CharacterMovementHandler>();
        characterUIHandler = GetComponent<PlayerCharacterUIHandler>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        weaponHandler = GetComponent<WeaponHandler>();
        hpHandler = GetComponent<HPHandler>();
        chatSystem = GetComponent<ChatSystem>();
        //if (HasStateAuthority || HasInputAuthority)
        //{
        //    //chatSystem = GetComponent<ChatSystem>();
        //}
        if (eventHandler == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }
        //Event
        chatSystem.SubscribeToPlayerActionEvents(eventHandler);
        movementHandler.SubscribeToPlayerActionEvents(eventHandler);
        playerStateHandler.SubscribeToPlayerActionEvents(eventHandler);
        hpHandler.SubscribeToPlayerActionEvents(eventHandler);
        weaponHandler.SubscribeToPlayerActionEvents(eventHandler);
        characterUIHandler.SubscribeToPlayerActionEvents(eventHandler);
        playerInfo.SubscribeToPlayerActionEvents(eventHandler);
        SubscribeToPlayerActionEvents(eventHandler);



        //if (HasInputAuthority)
        //{
        //    if (Runner.IsServer)
        //    {
        //        return;
        //    }
        //    chatSystem = GetComponent<ChatSystem>();
        //}

    }

    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        _playerActionEvents.OnGameEnd += OnGameEnd;
        _playerActionEvents.OnGameStart += OnGameStart;

    }

    void OnGameStart()
    {
        if (HasInputAuthority)
            GameManager.Instance.FadeIn_Out(1f);
    }

    void OnGameEnd()
    {
        if (HasInputAuthority)
            GameManager.Instance.FadeIn_Out(1f);
    }
    public override void FixedUpdateNetwork()
    {
        if (playerInfo == null)
        {
            playerInfo = GetComponent<PlayerInfo>();
            //Debug.Log("playerInfo == null");
            return;
        }

        if (HasStateAuthority || HasInputAuthority)
        {
            if (PlayAble())
            {
                if (GetInput(out NetworkInputData networkInputData))
                {
                    //Debug.Log("networkInputData");

                    //ShowBoard(networkInputData);
                    Chat(networkInputData);
                    if (playerStateHandler.canMove)
                    {
                        ActionMove(networkInputData);
                        ActionCase(networkInputData);
                    }
                }
            }
            if (eventHandler != null)
                eventHandler.TriggerCharacterUpdate();

        }

    }

    private void ActionCase(NetworkInputData networkInputData)
    {
        if (networkInputData.isJumpButtonPressed)
        {
            if (playerStateHandler.JumpAble())
            {
                eventHandler.TriggerJump();
                return;
            }
        }
        else
        {
            playerStateHandler.isJumpButtonPressed = false;
        }

        if (networkInputData.isFireButtonPressed)
        {
            if (playerStateHandler.AbleFire() && weaponHandler.AbleFire())
            {
                eventHandler.TriggerAttack();
                return;
            }

        }
        else
        {
            playerStateHandler.isFireButtonPressed = false;
        }



    }
    private void ActionMove(NetworkInputData networkInputData)
    {
        if (playerStateHandler.canMove)
        {

            Vector2 tmp = playerStateHandler.stopMove == true ? Vector2.zero : networkInputData.movementInput;
            eventHandler.TriggerMove(tmp.x);

        }
    }

    //UI
    //void ShowBoard(NetworkInputData networkInputData)
    //{

    //    if (networkInputData.isShowBoardButtonPressed)
    //    {
    //        //if(HasStateAuthority)
    //        //    _showBoard = !_showBoard;
    //    }
    //}
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
    bool PlayAble()
    {
        if (hpHandler == null)
        {
            Debug.Log("hpHandler is Null");
            return false;
        }
        if (eventHandler == null)
        {
            Debug.Log("eventHandler is Null");
            return false;
        }


        if (HasInputAuthority || HasStateAuthority)
        {
            if (hpHandler.isRespawnRequsted)
            {
                eventHandler.TriggerRespawn();

                return false;
            }
            if (hpHandler.isDead)
                return false;
        }
        playerInfo.playingState = GameManager.Instance.GetPlaying();
        if (playerInfo.playingState != (int)PlayingState.Stop)
        {
            return true;
        }
        return false;
    }



}
