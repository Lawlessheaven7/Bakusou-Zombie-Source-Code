using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatCanvas : MonoBehaviour
{
    public CanvasGroup chatCanvas;
    // Start is called before the first frame update
    void Start()
    {
       chatCanvas= GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(chatCanvas.alpha == 1)
            {
                canvasOff();
            }else if(chatCanvas.alpha == 0)
            {
                canvasOn();
            }
        }
    }

    public void canvasOn()
    {
        chatCanvas.alpha = 1;
    }

    public void canvasOff()
    {
        chatCanvas.alpha = 0;
    }
}
