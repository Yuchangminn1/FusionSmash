using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISize : MonoBehaviour
{
    public RectTransform rectTransform;
    public float witdth = 4f;
    public float height = 3f;

    public Vector2 screenSize;
    // UI ������Ʈ�� RectTransform ����

    

    private void Awake()
    {
        UIManager.Instance.uISizeChange += SetScreenSize;
    }


    public void SetScreenSize(Vector2 _screenSize)
    {
        //Vector3 currentPosition = rectTransform.position;

        screenSize = _screenSize;
        if (witdth != 0 && height != 0)
        {
            rectTransform.sizeDelta = new Vector2(screenSize.x / witdth, screenSize.y / height);
        }
    }
    
}
