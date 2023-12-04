using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class TestWeapon : MonoBehaviour
{
    string localPlayerName; // 자신의 이름 겹치지 않게

    bool directeAttack; //총알 같은 무기를 생각해서 직접 공격하는 무기만 적용되도록

    bool isHit = false;                // 데미지 줄 타이밍 네트워크 업데이트에 전달 

    public MeshCollider meshcol;
    List<GameObject> hitPlayer;
    private void Awake()
    {
        meshcol = GetComponent<MeshCollider>();
        hitPlayer = new List<GameObject>();
    }
    private void Start()
    {
        localPlayerName = transform.GetComponentInParent<NetworkPlayer>().nickName.Value;

        if (localPlayerName == null)
        {
            Debug.Log("이름이 없습니다");
            return;
        }
        Debug.Log(localPlayerName);
    }
    public void SetDirect(bool tf)
    {
        directeAttack = tf;
    }
    private void Update()
    {

    }
    public void WeaponColOn()
    {
        if (!directeAttack)
            return;
        meshcol.enabled = true;
        Debug.Log($"무기 콜라이더 {true}");
    }
    public void WeaponColOff()
    {
        if (!directeAttack)
            return;
        meshcol.enabled = false;
        Debug.Log($"무기 콜라이더 {false}");
    }
    public bool IsHit()
    {
        return isHit;
    }
    public void AttackPlayer()
    {
        if (hitPlayer.Count == 0)
        {
            Debug.Log("충돌한 플레이어가 없음 ");
            return;
        }
        int i = 0;
        
        foreach(GameObject player in hitPlayer)
        {
            Debug.Log($"{i} / {player.name}");
            player.GetComponent<HPHandler>().OnTakeDamage();
            ++i;
        }
        hitPlayer.Clear();
        isHit = false;
        Debug.Log("충돌 플레이어 리스트 클리어");
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

            if(localPlayerName != null)
            {
                if(localPlayerName!= other.transform.name)
                {
                    if (hitPlayer.Contains(other.gameObject)) //중복 방지
                        return;
                    hitPlayer.Add(other.gameObject);
                    isHit = true;

                    if (hitPlayer.Contains(other.gameObject))
                    {
                        Debug.Log($"{hitPlayer.IndexOf(other.gameObject)}번째 리스트 충돌한 플레이어 이름 = {other.gameObject.name}");
                    }

                }
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {

    //        if (localPlayerName != null)
    //        {
    //            if (localPlayerName != other.transform.name)
    //            {

    //                if (hitPlayer.Contains(other.gameObject))
    //                {
    //                    Debug.Log($"{hitPlayer.IndexOf(other.gameObject)}번째 리스트 삭제한 플레이어 이름 = {other.gameObject.name}");

    //                    hitPlayer.Remove(other.gameObject);
    //                }

    //            }
    //        }
    //        //Debug.Log("탈출");
    //    }
    //    Debug.Log("탈출");

    //}

}
