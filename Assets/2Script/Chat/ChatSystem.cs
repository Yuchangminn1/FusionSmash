using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.
using TMPro;
using Fusion;
using System.Xml;
using UnityEngine.InputSystem;
using System;

public class ChatSystem : NetworkBehaviour
{
    public PlayerInputAction playerControls;
    private InputAction sumit;

    [Networked(OnChanged = nameof(OnChangeChatLog))]
    public NetworkString<_16> myChat { get; set; }

    [Networked(OnChanged = nameof(OnChangeChatLog))]
    public NetworkString<_16> sendName { get; set; }

    //public TMP_Text myNameText;
    [Networked]
    public NetworkString<_16> _nickName { get; set; } = "";

    [SerializeField] InputField mainInputField;

    //bool isEnter = false;
    //bool chatOnOff = false;

    public TMP_Text chatLog;
    public Scrollbar scrollV;

    //bool chatDown = false;

    [Networked(OnChanged = nameof(OnChangeSumit))]
    public NetworkBool ischating { get; set; }

    //Ŭ���̾�Ʈ���� ���� onoffonoff�ݺ��ϴ°� ����
    //float inputTime = 0f;
    // Checks if there is anything entered into the input field.
    private void Start()
    {
        if (HasInputAuthority)
        {
            mainInputField = GameObject.Find("ChatInputField").GetComponent<InputField>();
            playerControls = new PlayerInputAction();
            chatLog = GameObject.Find("Chat_Display").GetComponentInChildren<TMP_Text>();
            scrollV = GameObject.Find("ScrollbarVertical").GetComponent<Scrollbar>();
            mainInputField.characterLimit = 1024;
        }

        

    }
    

    public override void Spawned()
    {

    }

    private void FixedUpdate()
    {
        
        if (HasInputAuthority)
        {
            if (scrollV)
                scrollV.value = 0;
        }
    }
    static void OnChangeSumit(Changed<ChatSystem> changed)
    {
        changed.Behaviour.Sumit(changed.Behaviour.ischating);
    }

    public void Sumit(bool ischating)
    {
        if (HasInputAuthority)
        {
            if (!ischating)
            {
                if (mainInputField.text != "" && mainInputField.text != " ")
                {
                    // ���� �� ��� 
                    Debug.Log(mainInputField.text.Length);
                    StartCoroutine(SumitE());
                }
                else
                {
                    mainInputField.interactable = false;
                }
            }
            else
            {
                mainInputField.interactable = true;
                mainInputField.Select();
            }
        }
    }
    public void IsChatChange()
    {
        if (HasStateAuthority)
            ischating = !ischating;
    }

    private void SendMassage()
    {
        myChat = mainInputField.text;
        chatLog.text += $"\n {_nickName.ToString()} : {myChat}";
        
        if (HasInputAuthority)
            RPC_SetChat(myChat.ToString(), _nickName.ToString());
        mainInputField.text = "";
        mainInputField.interactable = false;
        ischating = false;
    }

    static void OnChangeChatLog(Changed<ChatSystem> changed)
    {
        //Debug.Log("mychat = " + changed.Behaviour.myChat);
        if (changed.Behaviour.myChat == "")
        {
            return;
        }
        changed.Behaviour.PushMessage();

    }



    public void PushMessage()
    {

        string nullcheck = null;
        foreach (var chatSystem in myChat)
        {
            nullcheck += chatSystem;

            //Debug.Log($"�̰�{chatSystem}�̰�");
        }
        if (Object.HasInputAuthority)
        {
            scrollV.value = 0;
            return;
        }
        if (chatLog == null)
        {
            chatLog = GameObject.Find("Chat_Display").GetComponentInChildren<TMP_Text>();
        }

        chatLog.text += $"\n {sendName} : {myChat}";
        myChat = "";


    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetChat(string mychat, string _sendName, RpcInfo info = default)
    {
        sendName = _sendName;
        this.myChat = mychat;
    }

    //private void Sumit(InputAction.CallbackContext context)
    //{
    //    Debug.Log("chatDown change true ");
    //    chatDown = true;
    //}
    IEnumerator SumitE()
    {
        yield return new WaitForFixedUpdate();
        SendMassage();
    }

    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {
        _playerActionEvents.OnPlayerNameChange += OnPlayerNameChange;
    }

    public void OnPlayerNameChange(string _name)
    {
        _nickName = _name;
    }
}