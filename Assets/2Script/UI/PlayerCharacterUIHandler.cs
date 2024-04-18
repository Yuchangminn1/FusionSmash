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

    public TMP_Text name_Text;
    

    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {
        if (_playerActionEvents == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }
        //Death 
        _playerActionEvents.OnPlyaerTurn += OnPlyaerTurn;
        _playerActionEvents.OnPlayerNameChange += OnPlayerNameChange;
    }
    public void OnPlayerNameChange(string _name)
    {
        name_Text.text = _name;
        Debug.Log("NickName Change");
    }
    

    public void OnPlyaerTurn(float _pitch)
    {
        if (_pitch > 0)
        {
            name_Text.rectTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);

        }
        else if (_pitch < 0)
        {
            name_Text.rectTransform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        }
        
    }

}
