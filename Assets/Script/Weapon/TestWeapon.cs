using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : MonoBehaviour
{
    string localPlayerName;

    private void Start()
    {
        localPlayerName = transform.GetComponentInParent<NetworkPlayer>().nickName.Value;
        if(localPlayerName == null)
        {
            Debug.Log("이름이 없습니다");
            return;
        }
        Debug.Log(localPlayerName);
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

            if(localPlayerName != null)
            {
                if(localPlayerName!= other.transform.name)
                {
                    Debug.Log($" localPlayerName = {localPlayerName}  / other = {other.transform.name}");

                    other.transform.root.GetComponent<HPHandler>().OnTakeDamage();
                }
            }
        }
        //Debug.Log($"{other}");
    }
}
