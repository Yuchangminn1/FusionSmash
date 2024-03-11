using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-5, 5),4, 0);
    }

    public static void SetRenderLayerInChildren(Transform transform,int layerNumber)
    {
        foreach(Transform trans in transform.GetComponentsInChildren<Transform>(true)) 
        {
            if (trans.CompareTag("IgnoreLayerChange"))
            {
                //IgnoreLayerChange�� �ױ��̸� ���̾� ���� �������� �ٵ� ���� ���̾���� �Ƹ� ?�Ⱦ�
                continue;
            }
            trans.gameObject.layer = layerNumber;
        }
    }
}
