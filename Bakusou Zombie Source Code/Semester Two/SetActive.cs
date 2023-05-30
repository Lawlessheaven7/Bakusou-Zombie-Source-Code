using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetActive : MonoBehaviourPun
{
    public GameObject compass, compass2;
    // Start is called before the first frame update
    void Start()
    {
        if (ThirdPersonCameraControl.instance.myPV.IsMine)
        {
            compass.SetActive(true);
            compass2.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
