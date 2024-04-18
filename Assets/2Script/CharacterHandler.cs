using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Video.VideoPlayer;


public class CharacterHandler : NetworkBehaviour, IPlayerActionListener
{
    public PlayerActionEvents eventHandler;

    //Handler


    PlayerInfo playerInfo;
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
        CharacterGetComponent();

        EventSubscribe();

        

    }



    private void CharacterGetComponent()
    {
        playerInfo = GetComponent<PlayerInfo>();
        inputHandler = GetComponent<CharacterInputhandler>();
        movementHandler = GetComponent<CharacterMovementHandler>();
        characterUIHandler = GetComponent<PlayerCharacterUIHandler>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        weaponHandler = GetComponent<WeaponHandler>();
        hpHandler = GetComponent<HPHandler>();
        chatSystem = GetComponent<ChatSystem>();
    }

    


    public override void FixedUpdateNetwork()
    {
        if (playerInfo == null)
        {
            playerInfo = GetComponent<PlayerInfo>();
            Debug.Log("playerInfo == null");
            return;
        }

        if (HasStateAuthority || HasInputAuthority)
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                Chat(networkInputData);
                if (PlayAble())
                {
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
        //Debug.Log("PlayAble");
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

            if (hpHandler.isDead)
                return false;
        }
        if (playerInfo.PlayingState != (int)EPlayingState.Stop && playerInfo.PlayingState != (int)EPlayingState.Death)
        {
            return true;
        }
        return false;
    }

    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {

        //_playerActionEvents.OnPlayerJump += OnPlayerJump;
    }
    void OnPlayerJump()
    {
        
    }
    //void OnGameEnd()
    //{
    //    SetTraceCamera(true);
    //}
    
    //void OnPlyaerInit()
    //{
    //    SetTraceCamera(true);

    //}

    private void EventSubscribe()
    {
        eventHandler = GetComponent<PlayerActionEvents>();
        //Event
        SubscribeToPlayerActionEvents(ref eventHandler);
        chatSystem.SubscribeToPlayerActionEvents(ref eventHandler);
        movementHandler.SubscribeToPlayerActionEvents(ref eventHandler);
        playerStateHandler.SubscribeToPlayerActionEvents(ref eventHandler);
        hpHandler.SubscribeToPlayerActionEvents(ref eventHandler);
        weaponHandler.SubscribeToPlayerActionEvents(ref eventHandler);
        characterUIHandler.SubscribeToPlayerActionEvents(ref eventHandler);
        playerInfo.SubscribeToPlayerActionEvents(ref eventHandler);
    }
}
