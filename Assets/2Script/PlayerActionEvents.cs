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
    public event Action OnVictory;


    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action OnGameOver;



    public event Action<Vector3,int> OnPlayerKnockBack;
    public event Action<float> OnPlayerMove;
    public event Action<float> OnPlyaerTurn;
    public event Action<int> OnTakeDamage;
    public event Action<string> OnPlayerNameChange;




    public void TrigerFixedUpdate()
    {
        OnPlyaerFixedUpdate?.Invoke();
        //Debug.Log("TrigerFixedUpdate");

    }

    public void TriggerJump()
    {
        OnPlayerJump?.Invoke();
        Debug.Log("TriggerJump");

    }
    public void TriggerAttack()
    {
        OnPlayerAttack?.Invoke();
        Debug.Log("TriggerAttack");

    }
    public void TriggerCharacterUpdate()
    {
        OnPlayerUpdate?.Invoke();
        //Debug.Log("TriggerCharacterUpdate");

    }
    public void TriggerVictory()
    {
        OnVictory?.Invoke();
    }
    public void TriggerRespawn()
    {
        OnPlyaerRespawn?.Invoke();
        Debug.Log("TriggerRespawn");

    }
    public void TriggerInit()
    {
        OnPlyaerInit?.Invoke();
        Debug.Log("TriggerInit");

    }
    public void TriggerDeath()
    {
        OnPlyaerDeath?.Invoke();
        Debug.Log("TriggerDeath");

    }
    public void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        Debug.Log("TriggerGameOver");

    }
    public void TriggerPlayerKnockBack(Vector3 _dir, int _force)
    {
        OnPlayerKnockBack?.Invoke(_dir,_force);
    }
    public void TriggerMove(float direction)
    {
        OnPlayerMove?.Invoke(direction);
        //Debug.Log("TriggerMove");
    }
    public void TriggerPlyaerTurn(float _pitch)
    {
        OnPlyaerTurn?.Invoke(_pitch);
        //Debug.Log("TriggerPlyaerTurn");
    }
    public void TriggerPlayerNameChange(string _name)
    {
        OnPlayerNameChange?.Invoke(_name);
        Debug.Log("TriggerPlayerNameChange");
    }
    public void TriggerPlayerOnTakeDamage(int _force)
    {
        OnTakeDamage?.Invoke(_force);
        //Debug.Log("TriggerPlayerOnTakeDamage");
    }

    public void TriggerGameStart()
    {
        OnGameStart?.Invoke();
        Debug.Log("TriggerGameStart");

    }
    public void TriggerGameEnd()
    {
        OnGameEnd?.Invoke();
        Debug.Log("TriggerGameEnd");
    }


}
public interface IPlayerActionListener
{
    void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents);

}