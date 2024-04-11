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

    public void OnGameStart()
    {
        canvasPlaying.enabled = true;
        canvasWaiting.enabled = false;
    }
    public void OnGameEnd()
    {
        canvasPlaying.enabled = false;
        canvasWaiting.enabled = true;
    }

    public void OnGameWait()
    {
        canvasPlaying.enabled = false;
        canvasWaiting.enabled = true;
    }
}
