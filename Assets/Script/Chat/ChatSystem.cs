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
    public string sendName { get; set; }

    public TMP_Text myNameText;

    public string _nickName;

    [SerializeField] InputField mainInputField;

    bool isEnter = false;
    bool chatOnOff = false;

    public TMP_Text chatLog;
    public Scrollbar scrollV;

    bool chatDown = false;

    [Networked(OnChanged = nameof(OnChangeSumit))]
    public NetworkBool ischating { get; set; }

    //Ŭ���̾�Ʈ���� ���� onoffonoff�ݺ��ϴ°� ����
    float inputTime = 0f;
    // Checks if there is anything entered into the input field.
    public void Awake()
    {
        //�̰� �����ũ���� ���ϸ� null�̶� ������ 
        playerControls = new PlayerInputAction();
        chatLog = GameObject.FindWithTag("ChatDisplay").GetComponentInChildren<TMP_Text>();
        scrollV = GameObject.FindWithTag("ScrollV").GetComponent<Scrollbar>();
    }
    
    public void Start()
    {
        mainInputField.characterLimit = 1024;
        if (Object.HasInputAuthority || HasStateAuthority)
        {
            mainInputField.enabled = true;
        }
        //Debug.Log("chatLog = " + chatLog);

    }

    private void FixedUpdate()
    {
        scrollV.value = 0;
    }
    static void OnChangeSumit(Changed<ChatSystem> changed)
    {
        changed.Behaviour.Sumit(changed.Behaviour.ischating);
    }

    public void Sumit(bool ischating)
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
    public void IsChatChange()
    {
        if (HasStateAuthority)
            ischating = !ischating;
    }

    private void SendMassage()
    {
        myChat = mainInputField.text;
        chatLog.text += $"\n {_nickName} : {myChat}";
        RPC_SetChat(myChat.ToString(), _nickName);
        //Debug.Log($"Send MyChat = {myChat}");
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
    public override void Spawned()
    {
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
            return;
        }
        if (chatLog == null)
        {
            chatLog = GameObject.FindWithTag("ChatDisplay").GetComponent<TMP_Text>();
        }

        chatLog.text += $"\n {sendName} : {myChat}";
        myChat = "";
        scrollV.value = 0;

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetChat(string mychat, string _sendName, RpcInfo info = default)
    {

        sendName = _sendName;
        //Debug.Log($"[RPC] SetNickname : {mychat}");
        this.myChat = mychat;
    }

    private void OnEnable()
    {
        //Debug.Log("chatDown change true ");
        sumit = playerControls.Player.Sumit;
        sumit.Enable();

        sumit.performed += Sumit;
    }
    private void OnDisable()
    {
        sumit.Disable();
    }
    private void Sumit(InputAction.CallbackContext context)
    {
        Debug.Log("chatDown change true ");
        chatDown = true;
    }
    IEnumerator SumitE()
    {
        yield return new WaitForFixedUpdate();
        SendMassage();
    }
}