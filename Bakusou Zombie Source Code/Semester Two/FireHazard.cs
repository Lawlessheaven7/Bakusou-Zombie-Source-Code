using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class FireHazard : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Zombie")
        {
            photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("onFire", RpcTarget.All);
            }
        }

    }
}
