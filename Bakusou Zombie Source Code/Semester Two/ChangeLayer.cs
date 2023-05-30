using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeLayer : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerHost");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
