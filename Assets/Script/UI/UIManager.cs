using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    private static UIManager _instance;

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
                    Debug.Log("UIManager 없음");
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
    
    public void PlayerKDAScoreUI(int kill,int death)
    {
        textKDA.text = $"{kill}     /    {death}";
    }


    private void FixedUpdate()
    {
        
        //화면 비율 체크 
        if (screenSize.x != Screen.width*1f || screenSize.y != Screen.height * 1f)
        {
            uISizeChange(new Vector2(Screen.width * 1f, Screen.height * 1f));
        }
    }

    public void ScerrenSizeChange(Vector2 _screenSize) => screenSize = _screenSize;

    
    
}
