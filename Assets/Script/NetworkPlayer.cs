using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// �÷��̾� ��ȯ�� �� ���� �߰��ϰ������ ����ٰ� �߰��ϼ���
/// </summary>
public class NetworkPlayer : NetworkBehaviour,IPlayerLeft
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
        playerInfo = GetComponent<PlayerInfo>();
        if (Object.HasInputAuthority) // ���ӿ��� ������ �Ǹ� ? ������   > ������Ʈ�� �̵���ų �Ǹ�?  NetworkObject��ũ��Ʈ�� �ٿ��ָ� Ʈ��
        {
            Local = this;

            //Disable main camera ���� ī�޶� ����
            //Camera.main.gameObject.SetActive(false);
            if(PlayerPrefs.GetString("PlayerNickname") == "" || PlayerPrefs.GetString("PlayerNickname") == null)
            {
                RPC_SetNickName(PlayerPrefs.GetString("�г��� ��������"));

            }
            else
            {
                RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));
            }
            //Debug.Log("Spawned local player");

}
        else
        {
            //RPC_SetNickNameTOIn(PlayerPrefs.GetString("PlayerNickname"));
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            AudioListener audioListner = GetComponentInChildren<AudioListener>();
            audioListner.enabled = false;
            Canvas[] localCanvas = GetComponentsInChildren<Canvas>();
            foreach(Canvas c in localCanvas)
            {
                if(c.gameObject.tag == "Nickname")
                {
                    continue;
                }
                c.enabled = false;
            }

            Debug.Log("spawned remote player");
        }
        
        transform.name = transform.GetComponent<NetworkPlayer>().nickName.Value;

        

        //ü�¹� ��Ƽ�� ���̰� �ҷ��� ��ȣ �Ѱǵ� �� �����ε� �𸣰ڴ� �𸣰� ��
    }
    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.InputAuthority)
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
        //Debug.Log($"Nickname chaged for player to {nickName} for player {gameObject.name}");
        //characterHandler.playerdata.Name = nickName.ToString();
        playerInfo.SetName(nickName.ToString());




        characterMovementHandler._nickName = nickName.ToString();
        chatSystem._nickName = nickName.ToString();

    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        //Debug.Log($"[RPC] SetNickname : {nickName}");
        this.nickName = nickName;
    }
    
    
}
