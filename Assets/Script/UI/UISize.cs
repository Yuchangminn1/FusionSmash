using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISize : MonoBehaviour
{
    public RectTransform rectTransform;
    public float witdth = 4f;
    public float height = 3f;

    Vector2 screenSize;
    private void Awake()
    {
        UIManager.Instance.uISizeChange += SetScreenSize;
    }

    public void SetScreenSize(Vector2 _screenSize)
    {
        screenSize = _screenSize;
        if (witdth != 0 && height != 0)
        {
            rectTransform.sizeDelta = new Vector2(screenSize.x / witdth, screenSize.y / height);
        }
        Debug.Log("ÇÔ¼ö");
    }
}
