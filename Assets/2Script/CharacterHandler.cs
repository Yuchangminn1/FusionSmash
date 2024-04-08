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
        if(eventHandler)
            eventHandler.TrigerFixedUpdate();
    }
    public override void Spawned()
    {
        
        if (HasStateAuthority || HasInputAuthority)
        {
            //Input
            inputHandler = GetComponent<CharacterInputhandler>();
            movementHandler = GetComponent<CharacterMovementHandler>();

            playerStateHandler = GetComponent<PlayerStateHandler>();
            weaponHandler = GetComponent<WeaponHandler>();
            hpHandler = GetComponent<HPHandler>();
            chatSystem = GetComponent<ChatSystem>();
            characterUIHandler = GetComponent<PlayerCharacterUIHandler>();
            //Event
            eventHandler = GetComponent<PlayerActionEvents>();
            if (eventHandler == null)
            {
                Debug.LogError("PlayerActionEvents component is missing!");
                return;
            }
            movementHandler.SubscribeToPlayerActionEvents(eventHandler);
            playerStateHandler.SubscribeToPlayerActionEvents(eventHandler);
            hpHandler.SubscribeToPlayerActionEvents(eventHandler);
            characterUIHandler.SubscribeToPlayerActionEvents(eventHandler);
            weaponHandler.SubscribeToPlayerActionEvents(eventHandler);
        }
        if (HasInputAuthority)
        {
            if (Runner.IsServer)
            {
                return;
            }
            chatSystem = GetComponent<ChatSystem>();

        }
        
    }

    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        ;
    }
    public override void FixedUpdateNetwork()
    {
        
        if (HasStateAuthority ||HasInputAuthority)
        {
            if (PlayerAble())
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
                
            eventHandler.TriggerCharacterUpdate();
            //if (PlayerAble())
            //{


            //    if (GetInput(out NetworkInputData networkInputData))
            //    {
            //        //Debug.Log("networkInputData");

            //        //ShowBoard(networkInputData);
            //        Chat(networkInputData);
            //        if (playerStateHandler.canMove)
            //        {
            //            ActionMove(networkInputData);
            //            ActionCase(networkInputData);
            //        }
            //    }
            //    eventHandler.TriggerCharacterUpdate();
            //}
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
            if (playerStateHandler.AbleFire()&&weaponHandler.AbleFire() )
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
    bool PlayerAble()
    {
        if(hpHandler == null)
        {
            Debug.Log("hpHandler is Null");
        }
        if (eventHandler == null)
        {
            Debug.Log("eventHandler is Null");
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
        return true;
    }
}
