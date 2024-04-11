using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionEvents : MonoBehaviour
{
    //OnJump 했다가 NewInputSystem과 이름이 겹쳐서 에러남 
    public event Action OnPlayerJump;
    public event Action OnPlayerAttack;
    public event Action OnPlayerUpdate;
    public event Action OnPlyaerRespawn;
    public event Action OnPlyaerDeath;
    public event Action OnPlyaerInit;
    public event Action OnPlyaerFixedUpdate;


    public event Action OnGameStart;
    public event Action OnGameEnd;



    public event Action<float> OnPlayerMove;
    public event Action<float> OnPlyaerTurn;
    public event Action<int,bool> OnTakeDamage;
    public event Action<string> OnPlayerNameChange;




    public void TrigerFixedUpdate()
    {
        OnPlyaerFixedUpdate?.Invoke();
    }

    public void TriggerJump()
    {
        OnPlayerJump?.Invoke();
    }
    public void TriggerAttack()
    {
        OnPlayerAttack?.Invoke();
    }
    public void TriggerCharacterUpdate()
    {
        OnPlayerUpdate?.Invoke();
    }
    public void TriggerRespawn()
    {
        OnPlyaerRespawn?.Invoke();
    }
    public void TriggerInit()
    {
        OnPlyaerInit?.Invoke();
    }
    public void TriggerDeath()
    {
        OnPlyaerDeath?.Invoke();
    }
    public void TriggerMove(float direction)
    {
        OnPlayerMove?.Invoke(direction);
    }
    public void TriggerPlyaerTurn(float _pitch)
    {
        OnPlyaerTurn?.Invoke(_pitch);
        //Debug.Log("_pitch = " + _pitch);
    }
    public void TriggerPlayerNameChange(string _name)
    {
        OnPlayerNameChange?.Invoke(_name);

    }
    public void TriggerPlayerOnTakeDamage(int _force,bool _isSmash)
    {
        OnTakeDamage?.Invoke(_force, _isSmash);
    }

    public void TriggerGameStart()
    {
        OnGameStart?.Invoke();
    }
    public void TriggerGameEnd()
    {
        OnGameEnd?.Invoke();
    }


}
public interface IPlayerActionListener
{
    void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents);

}