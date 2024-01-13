using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAimAngle : MonoBehaviour
{
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetAngle();
    }

    public void SetAngle()
    {
        if (camera != null)
        {
            Quaternion tmp = camera.transform.rotation;
            tmp.y = transform.rotation.y;
            tmp.z = transform.rotation.z;
            //Debug.Log(tmp);
            transform.rotation = camera.transform.rotation;
        }
    }
}
