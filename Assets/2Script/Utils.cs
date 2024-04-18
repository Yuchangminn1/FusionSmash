using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-5f, 5f),4, 0);
    }
    public static Vector3 GameOverPoint()
    {
        return new Vector3(100f, -30f, 0f);
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
