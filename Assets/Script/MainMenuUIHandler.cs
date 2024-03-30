using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
//using UnityEditor.Build.Content;

public class MainMenuUIHandler : MonoBehaviour
{
    [Header("Panels")]
    public GameObject playerDetailsPanel;
    public GameObject sessionBrowserPanel;
    public GameObject createSessionPanel;
    public GameObject statusPanel;


    [Header("Players settings")]
    public TMP_InputField PlayerNameInputField;

    [Header("New game session")]
    public TMP_InputField sessionNameInputField;

    public string mySceneName = "MyS2";

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickName"))
        {
            PlayerNameInputField.text = PlayerPrefs.GetString("PlayerNickName");
        }
    }
    void HideAllPanel()
    {
        playerDetailsPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        statusPanel.SetActive(false);
    }
    public void OnFindGameClicked()
    {
        PlayerPrefs.SetString("PlayaerNickName", PlayerNameInputField.text);
        PlayerPrefs.Save();

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.OnJoinLobby();
        HideAllPanel();


        //GameManager.instance.playerNickName = inputField.text;

        sessionBrowserPanel.gameObject.SetActive(true);

        FindObjectOfType<SessionListUIHandler>(true).OnLookingForGameSession();


        //SceneManager.LoadScene(mySceneName);
    }

    public void OnCreateNewGameClicked()
    {
        HideAllPanel();

        createSessionPanel.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionNameInputField.text, mySceneName);

        HideAllPanel();

        statusPanel.gameObject.SetActive(true);
    }

    public void OnJoiningSever()
    {
        HideAllPanel();
        statusPanel.gameObject.SetActive(true);
    }

}
