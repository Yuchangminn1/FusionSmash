using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIArrow : MonoBehaviour
{
    RectTransform rectTransform;
    Vector2 scrrenSize;
    public Transform targetTransform;
    Vector2 dirVec = new Vector2(1, 0);
    Vector2 targetVec = new Vector2(0, 0);
    float Angle;
    Vector3 tmp = new Vector3();
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }
    private void OnEnable()
    {
        scrrenSize = transform.parent.GetComponent<RectTransform>().sizeDelta;
    }
    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (targetTransform != null)
        {
            if (targetTransform.position.y > 0)
            {
                targetVec.x = targetTransform.position.x;
                targetVec.y = targetTransform.position.y;
                Angle = Vector2.Angle(dirVec, targetVec.normalized);

                tmp.x = 0;
                tmp.y = 0;
                tmp.z = Angle - 90f;
                rectTransform.localEulerAngles = tmp;
                Angle *= Mathf.Deg2Rad;
                tmp.x= scrrenSize.x/(2f*1.2f) * (float)Math.Cos(Angle); tmp.y= scrrenSize.y/ 1.2f * (float)Math.Sin(Angle); tmp.z =0;
                rectTransform.anchoredPosition = tmp;
                
                Debug.Log($"Angle =  {Angle * Mathf.Rad2Deg}  / Position = [{tmp.x} / {tmp.y}]");
            }
        }
            
    }
    
}
