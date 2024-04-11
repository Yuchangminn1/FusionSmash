using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CMCameraTest : MonoBehaviour
{
    //

    public Transform plyaerTransform;
    public Vector3 DisVector;

    float MinY = 12;
    float MaxX = 15;
    float disDefaultZ = -5;
    float disMaxZ = -10;

    private void LateUpdate()
    {
        if (plyaerTransform)
        {
            float absx = Math.Abs(transform.position.x);
            if (absx > MaxX)
            {
                float disZ = disDefaultZ - (absx / 2f);
                Vector3 tmp = DisVector;
                tmp.z = disZ;
                DisVector = Vector3.Lerp(DisVector, tmp, 0.2f);

            }
            else
            {
                Vector3 tmp = DisVector;
                tmp.z = disDefaultZ;
                DisVector = Vector3.Lerp(DisVector, tmp, 0.01f);
            }
            if (plyaerTransform.position.y > -MinY)
                transform.position = Vector3.Lerp(transform.position, plyaerTransform.position + DisVector, 0.05f);
        }
    }
}
