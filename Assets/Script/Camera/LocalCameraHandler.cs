using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : NetworkBehaviour
{
    public Transform cameraAnchorPoint;


    Vector2 viewInput;

    public float cameraRotationX { get; private set; } = 0;
    public float cameraRotationY { get; private set; } = 0;

    public float sensitivity { get; set; } =  1f;


    CharacterMovementHandler characterMovementHandler;

    Camera localCamera;

    void Awake()
    {
        localCamera = GetComponent<Camera>();
        characterMovementHandler = GetComponentInParent<CharacterMovementHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!HasInputAuthority)
        {
            localCamera.enabled = false;
        }
        else
        {
            localCamera.transform.parent = null;
        }
    }

    void LateUpdate()
    {
        LocalCameraRotation();
    }

    public void LocalCameraRotation()
    {
        //���� ī�޶� ȸ�� 
        if (cameraAnchorPoint == null)
        {
            return;
        }
        //if (!localCamera.enabled)
        //{
        //    return;
        //}
        //Move the camera to the position of the player
        localCamera.transform.position = cameraAnchorPoint.position;

        //Calculate rotation
        //cameraRotationX += viewInput.y * Time.deltaTime * characterMovementHandler.viewUpDownRotationSpeed;
        //cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        //cameraRotationY += viewInput.x * Time.deltaTime * characterMovementHandler.cameraRotationSpeed;


        
       // localCamera.transform.rotation = Quaternion.Euler(cameraRotationX / 2 * sensitivity, cameraRotationY / 2 * sensitivity, 0);

    }
    public Rotation CMLookRotation() 
    {
        return gameObject.transform.rotation;
    } 


    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
    
}
