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
    //New Input System;
    public PlayerInputAction playerControls;
    private InputAction chat;



    [Networked(OnChanged = nameof(OnChangeChatLog))]
    public NetworkString<_16> myChat { get; set; }

    [Networked(OnChanged = nameof(OnChangeChatLog))]
    public string sendName { get; set; }

    public TMP_Text sendPlayer;

    public string myName;


    //public bool logChange = false;

    [SerializeField] InputField mainInputField;

    bool isEnter = false;
    bool chatOnOff = false;


    //bool repit = false;

    //Debug.Log($"Push {nowString}");
    //TMPText.text += nowString;

    public TMP_Text chatLog;
    public Scrollbar scrollV;

    bool chatDown = false;



    //클라이언트에서 쳇이 onoffonoff반복하는거 방지
    float inputTime = 0f;
    // Checks if there is anything entered into the input field.
    public void Awake()
    {
        //이걸 어웨이크에서 안하면 null이라 오류남 
        playerControls = new PlayerInputAction();

    }
    public void Start()
    {


        mainInputField.characterLimit = 1024;
        if (Object.HasInputAuthority)
        {
            mainInputField.enabled = true;
        }
        chatLog = GameObject.FindWithTag("ChatDisplay").GetComponent<TMP_Text>();
        Debug.Log("chatLog = " + chatLog);
        scrollV = GameObject.FindWithTag("ScrollV").GetComponent<Scrollbar>();



        //sendPlayer = PlayerPrefs.GetString("PlayerNickname");
    }

    //public override void FixedUpdateNetwork()
    //{
    //    if (Object.HasInputAuthority)
    //    {
    //        if (GetInput(out NetworkInputData networkInputData))
    //        {
    //            if (networkInputData.isRightEnterPressed)
    //            {

    //                chatDown = true;
    //            }

    //        }
    //    }

    //}
    public override void FixedUpdateNetwork()
    {
        //if (chatDown)
        //{
        //    Summit();
        //    Debug.Log("Enter로  Summit 실행");
        //    chatDown = false;
        //}
    }
    private void Update()
    {
        //if (Object.HasInputAuthority)
        //{
        //if (Input.GetButtonDown("Submit"))
        //{
        //    chatDown = true;
        //}



        //}
    }
    private void FixedUpdate()
    {
        myName = sendPlayer.text;
        sendName = myName;
        if (chatDown)
        {
            Debug.Log("chatDown");

            Summit();
            Debug.Log("Enter로  Summit 실행");
            chatDown = false;
        }
    }
    public void Summit()
    {

        if (mainInputField.interactable)
        {
            if (mainInputField.text != "" && mainInputField.text != " ")
            {
                myChat = mainInputField.text;
                chatLog.text += $"\n {myName} : " + myChat;

                RPC_SetChat(myChat.ToString(), sendName);
                Debug.Log($"Send MyChat = {myChat}");
                mainInputField.text = "";
            }
            mainInputField.interactable = false;

        }
        else
        {
            Debug.Log("채팅 활성화");
            mainInputField.interactable = true;
            mainInputField.Select();
        }

    }

    //private void FixedUpdate()
    //{

    //    if (chatDown)
    //    {

    //        //scrollV.value = 0; 에를 네트워크로 해서 변경될 떄 함수 호출해보자 ㅇㅇ
    //        Debug.Log("chatDown");
    //        scrollV.value = 0;
    //        chatDown = false;
    //    }
    //}

    static void OnChangeChatLog(Changed<ChatSystem> changed)
    {
        Debug.Log("mychat = " + changed.Behaviour.myChat);
        if (changed.Behaviour.myChat == "")
        {
            return;
        }
        //changed.Behaviour.PushMessageName();

        changed.Behaviour.PushMessage();
        //Debug.Log("이름은 " +changed.Behaviour.transform.name);
    }


    public void PushMessage()
    {
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
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetChat(string mychat, string _sendName, RpcInfo info = default)
    {
        sendName = _sendName;
        Debug.Log($"[RPC] SetNickname : {mychat}");
        this.myChat = mychat;
    }

    private void OnEnable()
    {
        Debug.Log("chatDown change true ");
        chat = playerControls.Player.Chat;
        chat.Enable();

        chat.performed += Chat;
    }
    private void OnDisable()
    {
        chat.Disable();
    }
    private void Chat(InputAction.CallbackContext context)
    {
        Debug.Log("chatDown change true ");
        chatDown = true;
    }
}