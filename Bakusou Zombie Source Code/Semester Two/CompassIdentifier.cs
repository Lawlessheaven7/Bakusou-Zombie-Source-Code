using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CompassIdentifier : MonoBehaviourPun
{
    public GameObject HNSCompass;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            HNSCompass.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
