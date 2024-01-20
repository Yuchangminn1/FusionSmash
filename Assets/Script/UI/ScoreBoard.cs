using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : NetworkBehaviour
{
    public int _playerNum = 0;
    public TMP_Text _scoretext;
    // Start is called before the first frame update
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        PlayerNumberUpdate();
    }
    public void PlayerNumberUpdate()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        _playerNum = players.Length;
        _scoretext.text = "";
        foreach (GameObject player in players)
        {
            CharacterMovementHandler tmp = player.GetComponent<CharacterMovementHandler>();
            
            //_scoretext.text += $"{player.GetComponent<NetworkPlayer>().nickName.ToString()}        {tmp._kill}  /  {tmp._death}  \n";
        }
    }
}
