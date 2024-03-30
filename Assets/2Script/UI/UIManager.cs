using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    
    private static UIManager _instance;

    public List<GameObject> UIScore = new List<GameObject>();
    public List<GameObject> UIChat = new List<GameObject>();


    Vector2 screenSize;
    public delegate void UISizeChange(Vector2 _screenSize);
    public UISizeChange uISizeChange;
    public TMP_Text textKDA;
    

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    Debug.Log("UIManager Is Null");
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        uISizeChange = new UISizeChange(ScerrenSizeChange);
    }

    private void Start()
    {
        UIScore.Add(GameObject.Find("KDA"));
        UIChat.Add(GameObject.Find("Chat_Display"));
        UIChat.Add(GameObject.Find("InputField"));
    }

    public void PlayerKDAScoreUI(int kill,int death)
    {
        textKDA.text = $"{kill}     /    {death}";
    }


    private void FixedUpdate()
    {
        
        //UI Size Change
        if (screenSize.x != Screen.width*1f || screenSize.y != Screen.height * 1f)
        {
            uISizeChange(new Vector2(Screen.width * 1f, Screen.height * 1f));
        }
    }

    public void ScerrenSizeChange(Vector2 _screenSize) => screenSize = _screenSize;

    
    
}
