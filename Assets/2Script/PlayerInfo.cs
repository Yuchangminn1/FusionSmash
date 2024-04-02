using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [Networked(OnChanged = nameof(NickNameChanged))]
    public NetworkString<_16> playerName { get; set; }
    [Networked(OnChanged = nameof(EnemyNameChanged))]
    public NetworkString<_16> enemyName { get; set; }
    //public string Name { get; set; }
    [Networked(OnChanged = nameof(ChangeKill))]
    public int kill { get; set; } = 0;
    [Networked(OnChanged = nameof(ChangeDeath))]
    public int death { get; set; } = 0;
    [Networked]
    public int playerNumber { get; set; }
    PlayerInfoUIManager playerInfoUIManager;

    //public PlayerInfo enemyinfo;
    public override void Spawned()
    {
        PlayerInfo[] players = FindObjectsOfType<PlayerInfo>();
        playerNumber = players.Length;

        //playerInfoUIManager = GameObject.Find("PlayersInfos").GetComponent<PlayerInfoUIManager>();
        //if (playerInfoUIManager != null)
        //{
        //    playerInfoUIManager.AddPlayerInfo(playerNumber, Name);
        //}
    }
    
    static void NickNameChanged(Changed<PlayerInfo> changed)
    {
        changed.Behaviour.SetName(changed.Behaviour.playerName.ToString());
    }
    static void EnemyNameChanged(Changed<PlayerInfo> changed)
    {
        changed.Behaviour.SetEnemyName(changed.Behaviour.enemyName.ToString());
    }
    
    static void ChangeKill(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.kill;
        changed.LoadOld();
        int oldS = changed.Behaviour.kill;
        if (newS != oldS)
        {
            changed.Behaviour.SetKill(newS);
        }
    }
    static void ChangeDeath(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.death;
        changed.LoadOld();
        int oldS = changed.Behaviour.death;
        if (newS != oldS)
        {
            changed.Behaviour.SetDeath(newS);
        }
    }
    public void SetName(string _nickName)
    {
        playerName = _nickName;
        HPHandler hp = GetComponent<HPHandler>();
        hp.nickNameText.text = _nickName;
    }
    public void SetEnemyName(string _enemyName)
    {
        enemyName = _enemyName;
    }
    public string GetName()
    {
        return playerName.ToString();
    }
    public string GetEnemyName()
    {
        return enemyName.ToString();
    }
    public void SetKill(int _kill)
    {
        kill = _kill;
    }
    public void SetDeath(int _death)
    {
        death = _death;
    }

    public void OnRespawned()
    {
        ++death;
        SetEnemyName("");
    }


}
