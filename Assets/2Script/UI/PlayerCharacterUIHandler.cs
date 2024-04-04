using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Fusion;
using System;

public class PlayerCharacterUIHandler : MonoBehaviour
{
    public PlayerActionEvents actionEvents;

    public TMP_Text name_Text;
    //public Sprite playerNum_Sprite;
    //public Sprite[] spirte;

    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        if (_playerActionEvents == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }
        //Death 
        _playerActionEvents.OnPlyaerTurn += NickNameRotation;
        
    }
    
    //public void SetCharacterImage(int _imageNum)
    //{
    //    playerNum_Sprite = spirte[_imageNum];
    //}

    public void NickNameRotation(float _pitch)
    {
        if (_pitch > 0)
        {
            name_Text.rectTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);

            //Debug.Log("_pitch =" + _pitch);
        }
        else if (_pitch < 0)
        {
            name_Text.rectTransform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            //Debug.Log("_pitch =" + _pitch);

        }
        //Quaternion targetRotation = Quaternion.identity;
        //targetRotation.y = -_pitch;
        //_pitch *= Mathf.Rad2Deg;
        //Debug.Log("targetRotation.y = -_pitch;" + _pitch);
        //name_Text.rectTransform.rotation = targetRotation;
    }

}
