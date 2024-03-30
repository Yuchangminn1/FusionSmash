using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class ChatDisPlayHandelr : MonoBehaviour
{



    public Scrollbar scrollbarV;
    bool donotouch = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (scrollbarV.value != 0 && !donotouch)
        {
            scrollbarV.value = 0;
        }
        
    }
}
