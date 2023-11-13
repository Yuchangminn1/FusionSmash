using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

/// <summary>
/// 플레이어 소환할 때 뭔가 추가하고싶으면 여기다가 추가하세용
/// </summary>
public class NetworkPlayer : NetworkBehaviour,IPlayerLeft
{
    public TextMeshProUGUI playerNickNameTM;
    public static NetworkPlayer Local { get; set; }
    public int mySNum = 0;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }



    //특정 부분 안보이게
    //public Transform playerModel;

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority) // 게임에서 움직일 권리 ? 같은거   > 오브젝트를 이동시킬 권리?  NetworkObject스크립트를 붙여주면 트루
        {
            Local = this;

            //Disable main camera 메인 카메라 제거
            //Camera.main.gameObject.SetActive(false);

            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));
            Debug.Log("Spawned local player");
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
        //이름 Clone말고 번호로 하는법
        //string Q = $"{Object.Id}";
        //transform.name = PlayerPrefs.GetString("PlayerNickname");
        //Q =$"{Q[4]}";
        //mySNum = int.Parse(Q);
        //Debug.Log(mySNum);

        //체력바 파티로 보이게 할려고 번호 한건디 흠 별로인듯 모르겠다 모르겠 어
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
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.nickName}");
        changed.Behaviour.OnNickNameChanged();
    }
    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname chaged for player to {nickName} for player {gameObject.name}");

        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickname : {nickName}");
        this.nickName = nickName;
    }
    //[Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    //public void RPC_SetNickNameTOIn(string nickName, RpcInfo info = default)
    //{
    //    Debug.Log($"[RPC] SetNickname : {nickName}");
    //    this.nickName = nickName;
    //}
}
