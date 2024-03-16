using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct PlayerData
{
    public string Name { get;  set; }
    public string EnemyName { get;  set; }
    public int Weapon { get;  set; }
    public int Kill { get;  set; }
    public int Death { get;  set; }

}

public class CharacterHandler : NetworkBehaviour
{
    PlayerData playerdata;
    public event Action<CharacterHandler> Jump;
    public event Action<CharacterHandler> Attack;
    public event Action<CharacterHandler> CharacterUpdate;
    public event Action<CharacterHandler> Respawn;

    public event Action<Vector2> Move;

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

        //Vind
        playerStateHandler.ActionVind(this);
        movementHandler.ActionVind(this, hpHandler);
        hpHandler.ActionVind(this);

        playerdata.Name = "임시";
        playerdata.EnemyName = "";
        playerdata.Kill = 0;
        playerdata.Death = 0;
        playerdata.Weapon = 1;

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
                if (Attack != null)
                {
                    Attack(this);
                }
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
