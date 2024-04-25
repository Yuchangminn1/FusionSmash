using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField]
    Camera playerTraceCamera;
    [SerializeField]
    Camera observingCamera;
    Vector3 endingPos = new Vector3(3.5f, 1.8f, -5f);
    Vector3 observingPos = new Vector3(2.5f, 2f, -9f);

    private void Awake()
    {
        //Camera
        playerTraceCamera = GameObject.Find("TraceCamera").GetComponent<Camera>();
        observingCamera = GameObject.Find("ObservingCamera").GetComponent<Camera>();
    }
    // Start is called before the first frame update

    public void SetTraceCamera(bool _tf)
    {
        observingCamera.transform.localPosition = endingPos;
        playerTraceCamera.enabled = _tf;
        observingCamera.enabled = !_tf;
        Debug.Log("Trace Camera Set");
    }

    public void SetEndingCamera()
    {
        observingCamera.transform.localPosition = endingPos;

    }

}
