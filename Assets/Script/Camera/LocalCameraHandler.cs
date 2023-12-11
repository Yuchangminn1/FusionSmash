using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    Vector2 viewInput;

    float cameraRotationX = 0;
    float cameraRotationY = 0;

    //Other component
    CharacterMovementHandler characterMovementHandler;
    //NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    Camera localCamera;

    void Awake()
    {
        localCamera = GetComponent<Camera>();
        characterMovementHandler = GetComponentInParent<CharacterMovementHandler>();
        //networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }
    }

    void LateUpdate()
    {
        if(cameraAnchorPoint == null)
        {
            return;
        }
        if (!localCamera.enabled)
        {
            return;
        }
        //Move the camera to the position of the player
        localCamera.transform.position = cameraAnchorPoint.position;

        //Calculate rotation
        cameraRotationX += viewInput.y * Time.deltaTime * characterMovementHandler.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime * characterMovementHandler.rotationSpeed;


        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX/2, cameraRotationY/2, 0);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
    
}
