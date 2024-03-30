using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class TestWeapon : MonoBehaviour
{
    string localPlayerName; // �ڽ��� �̸� ��ġ�� �ʰ�

    bool directeAttack; //�Ѿ� ���� ���⸦ �����ؼ� ���� �����ϴ� ���⸸ ����ǵ���

    bool isHit = false;                // ������ �� Ÿ�̹� ��Ʈ��ũ ������Ʈ�� ���� 

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
            Debug.Log("�̸��� �����ϴ�");
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
        //if (!directeAttack)
        //    return;
        meshcol.enabled = true;
        Debug.Log($"���� �ݶ��̴� {true}");
    }
    public void WeaponColOff()
    {
        //if (!directeAttack)
        //    return;
        meshcol.enabled = false;
        Debug.Log($"���� �ݶ��̴� {false}");
    }
    public bool IsHit()
    {
        return isHit;
    }
    //public void AttackPlayer()
    //{
    //    if (hitPlayer.Count == 0)
    //    {
    //        Debug.Log("�浹�� �÷��̾ ���� ");
    //        return;
    //    }
    //    int i = 0;
        
    //    foreach(GameObject player in hitPlayer)
    //    {
    //        Debug.Log($"{i} / {player.name}");
    //        player.GetComponent<HPHandler>().OnTakeDamage();
    //        ++i;
    //    }
    //    hitPlayer.Clear();
    //    isHit = false;
    //    Debug.Log("�浹 �÷��̾� ����Ʈ Ŭ����");
    //}
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

            if(localPlayerName != null)
            {
                if(localPlayerName!= other.transform.name)
                {
                    if (hitPlayer.Contains(other.gameObject)) //�ߺ� ����
                        return;
                    hitPlayer.Add(other.gameObject);
                    isHit = true;

                    if (hitPlayer.Contains(other.gameObject))
                    {
                        Debug.Log($"{hitPlayer.IndexOf(other.gameObject)}��° ����Ʈ �浹�� �÷��̾� �̸� = {other.gameObject.name}");
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
    //                    Debug.Log($"{hitPlayer.IndexOf(other.gameObject)}��° ����Ʈ ������ �÷��̾� �̸� = {other.gameObject.name}");

    //                    hitPlayer.Remove(other.gameObject);
    //                }

    //            }
    //        }
    //        //Debug.Log("Ż��");
    //    }
    //    Debug.Log("Ż��");

    //}

}
