using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterHandler : NetworkBehaviour
{
    public static event Action<CharacterHandler> Jump;
    public static event Action<CharacterHandler> Attack;
    public static event Action<CharacterHandler> CharacterUpdate;
    public static event Action<CharacterHandler> Respawn;

    public static event Action<Vector2> Move;

    //Handler
    CharacterInputhandler inputhandler;
    CharacterMovementHandler movementHandler;
    PlayerStateHandler playerStateHandler;
    HPHandler hpHandler;
    ChatSystem chatSystem;
    public override void Spawned()
    {
        //Input
        inputhandler = GetComponent<CharacterInputhandler>();
        movementHandler = GetComponent<CharacterMovementHandler>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        hpHandler = GetComponent<HPHandler>();
        chatSystem = GetComponent<ChatSystem>();
    }
    public override void FixedUpdateNetwork()
    {
        if (PlayerAble())
        {
            if (GetInput(out NetworkInputData networkInputData))
            {
                ShowBoard(networkInputData);
                Chat(networkInputData);
                ActionMove(networkInputData);
                ActionCase(networkInputData);
            }
            if(CharacterUpdate!=null)
                CharacterUpdate(this);
        }
    }
    
    private void ActionCase(NetworkInputData networkInputData)
    {
        if (Jump != null)
        {
            if (networkInputData.isJumpButtonPressed)
            {
                if (playerStateHandler.JumpAble())
                {
                    Jump(this);
                    return;
                }

            }
        }
        if (networkInputData.isFireButtonPressed)
        {
            if (playerStateHandler.AbleFire())
            {
                //Attack(this);
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
