using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    public Canvas canvasBG;
    public Canvas canvasPlaying;
    public Canvas canvasWaiting;
    public TMP_Text winnerText;
    public GameObject[] playerInfoObjects;
    public PlayerInfoUI[] playerInfoUIs;

    private static UIManager _instance;

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
    }
    private void Start()
    {
        foreach (GameObject playerInfoObject in playerInfoObjects)
        {
            playerInfoObject.SetActive(false);
        }
    }
    public void SetUIObject(int _num, bool _OnOff)
    {
        playerInfoObjects[_num - 1].SetActive(_OnOff);
    }
    public PlayerInfoUI GetUIObject(int _num)
    {
        return playerInfoUIs[_num - 1];
    }
    public void OnGameStart()
    {
        winnerText.enabled = false;
        canvasPlaying.enabled = true;
        canvasWaiting.enabled = false;
    }
    public void OnGameEnd()
    {
        winnerText.enabled = true;
        canvasPlaying.enabled = false;
        canvasWaiting.enabled = false;
    }

    public void OnGameWait()
    {
        winnerText.enabled = false;
        canvasPlaying.enabled = false;
        canvasWaiting.enabled = true;
    }
}
