using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// �÷��̾� ��ȯ�� �� ���� �߰��ϰ������ ����ٰ� �߰��ϼ���
/// </summary>
public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TMP_Text playerNickNameTM;
    PlayerInfo playerInfo;
    public static NetworkPlayer Local { get; set; }
    public int mySNum = 0;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    CharacterMovementHandler characterMovementHandler;
    HPHandler hphandler;
    ChatSystem chatSystem;

    //Ư�� �κ� �Ⱥ��̰�
    //public Transform playerModel;

    // Start is called before the first frame update

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hphandler = GetComponent<HPHandler>();
        chatSystem = GetComponent<ChatSystem>();
    }

    void Start()
    {

    }
    
    public override void Spawned()
    {
        Debug.Log("NetworkPlayer Spawned");
        playerInfo = GetComponent<PlayerInfo>();
        if (Object.HasInputAuthority) // ���ӿ��� ������ �Ǹ� ? ������   > ������Ʈ�� �̵���ų �Ǹ�?  NetworkObject��ũ��Ʈ�� �ٿ��ָ� Ʈ��
        {
            Debug.Log("NetworkPlayer Spawned HasInputAuthority");

            Local = this; 

            if (PlayerPrefs.GetString("PlayaerNickName") == "" || PlayerPrefs.GetString("PlayaerNickName") == null)
            {
                RPC_SetNickName(PlayerPrefs.GetString("PlayaerNickName"));

            }
            else
            {
                nickName = PlayerPrefs.GetString("PlayaerNickName");
                RPC_SetNickName(nickName.ToSafeString());
            }

        }
        else
        {

        }

        transform.name = transform.GetComponent<NetworkPlayer>().nickName.Value;

    }
    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        //Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.nickName}");
        changed.Behaviour.OnNickNameChanged();
    }
    private void OnNickNameChanged()
    {
        playerInfo.SetName(nickName.ToString());
        characterMovementHandler._nickName = nickName.ToString();
        chatSystem._nickName = nickName.ToString();

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        //Debug.Log($"[RPC] SetNickname : {nickName}");
        this.nickName = nickName;
    }


}
