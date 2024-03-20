using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Video.VideoPlayer;



public class CharacterHandler : NetworkBehaviour
{
    //Input 
    public event Action Jump;
    public event Action Attack;
    public event Action<CharacterHandler> CharacterUpdate;
    public event Action<CharacterHandler> Respawn;

    public event Action<Vector2> Move;

    //Handler
    EventHandler eventHandler;
    CharacterInputhandler inputhandler;
    CharacterMovementHandler movementHandler;
    PlayerStateHandler playerStateHandler;
    WeaponHandler weaponHandler;
    HPHandler hpHandler;
    ChatSystem chatSystem;
    public override void Spawned()
    {
        //Input
        eventHandler = GetComponent<EventHandler>();
        inputhandler = GetComponent<CharacterInputhandler>();
        movementHandler = GetComponent<CharacterMovementHandler>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        weaponHandler = GetComponent<WeaponHandler>();
        hpHandler = GetComponent<HPHandler>();
        chatSystem = GetComponent<ChatSystem>();

        //Vind
        playerStateHandler.ActionVind(this);
        movementHandler.ActionVind(this, hpHandler);
        weaponHandler.ActionVind(this);
        hpHandler.ActionVind(this);



    }

    public override void FixedUpdateNetwork()
    {
        if (PlayerAble())
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                ShowBoard(networkInputData);
                Chat(networkInputData);
                if (playerStateHandler.canMove)
                {
                    ActionMove(networkInputData);
                    ActionCase(networkInputData);
                }
            }
            if (CharacterUpdate == null)
            {
                Debug.Log("CharacterUpdate is null");
            }
            else
            {
                CharacterUpdate(this);
            }
        }
    }

    private void ActionCase(NetworkInputData networkInputData)
    {

        if (networkInputData.isJumpButtonPressed)
        {
            if (playerStateHandler.JumpAble())
            {
                Jump?.Invoke();
                return;
            }

        }

        if (networkInputData.isFireButtonPressed)
        {
            if (playerStateHandler.AbleFire())
            {
                Attack?.Invoke();
                return;
            }
        }



    }
    private void ActionMove(NetworkInputData networkInputData)
    {
        if (Move != null)
        {
            if (playerStateHandler.canMove)
            {
                //Vector2 tmp = networkInputData.movementInput;

                Vector2 tmp = playerStateHandler.stopMove == true ? Vector2.zero : networkInputData.movementInput;
                Move(tmp);
            }
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
    bool PlayerAble()
    {
        if (HasStateAuthority)
        {
            if (hpHandler.isRespawnRequsted)
            {
                if (Respawn != null)
                {
                    Respawn(this);
                    return false;
                }
            }
            if (hpHandler.isDead)
                return false;
        }
        return true;
    }
}
