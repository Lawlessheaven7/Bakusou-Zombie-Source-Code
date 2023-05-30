using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class SpeedBoost : MonoBehaviourPun
{
    public static SpeedBoost instance;

    public bool triggerentered;
    public bool player;
    public bool zombie;

    private PhotonView pv;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (other.gameObject.tag == "Player")
        {
            photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("speedBoost", RpcTarget.All);
            }
        }

        if (other.gameObject.tag == "Zombie")
        {
            photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("speedBoost", RpcTarget.All);
            }
        }
    }
}
