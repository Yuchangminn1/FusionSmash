using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-20, 20),4, Random.Range(-20, 20));
    }

    public static void SetRenderLayerInChildren(Transform transform,int layerNumber)
    {
        foreach(Transform trans in transform.GetComponentsInChildren<Transform>(true)) 
        {
            if (trans.CompareTag("IgnoreLayerChange"))
            {
                //IgnoreLayerChange가 테그이면 레이어 변경 하지마라 근데 나는 레이어변경을 아마 ?안씀
                continue;
            }
            trans.gameObject.layer = layerNumber;
        }
    }
}
