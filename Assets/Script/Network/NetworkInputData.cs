using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public Vector3 aimFowardVector;
    public NetworkBool isJumpButtonPressed;
    public NetworkBool isFireButtonPressed;
    public NetworkBool isRightEnterPressed;

    public int fireNum;


}
