using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviourPunCallbacks
{

    public int damage = 25;

    public bool damageZombie, damagePlayer;
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
        if (other.gameObject.tag == "Player" && damagePlayer)
        {
            other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, damage, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        if (other.gameObject.tag == "Zombie" && damageZombie)
        {
            other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, damage, PhotonNetwork.LocalPlayer.ActorNumber);
        }

    }
}

