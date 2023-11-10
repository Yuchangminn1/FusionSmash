using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : NetworkBehaviour
{
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.HasKey("PlayerNickname"))
            inputField.text = PlayerPrefs.GetString("PlayerNickname");

    }


    public void OnJoinGameCliked()
    {
        PlayerPrefs.SetString("PlayerNickname", inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("World1");
    }

}
