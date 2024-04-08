using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMCameraTest : MonoBehaviour
{
    public Transform plyaerTransform;
    public Vector3 DisVector;


    private void LateUpdate()
    {
        if (plyaerTransform)
        {
            if(Math.Abs(plyaerTransform.position.y) < 12)
                transform.position = Vector3.Lerp(transform.position, plyaerTransform.position + DisVector, 0.05f);
        }
    }
}
