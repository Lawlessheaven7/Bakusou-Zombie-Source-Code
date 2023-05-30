using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class damageblock : MonoBehaviourPunCallbacks
{

    public bool damagePlayer, damageZombie;
    public int hitDamage;
    public GameObject hitEffect;
    public Transform palm;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && damagePlayer)
        {
            other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, hitDamage, PhotonNetwork.LocalPlayer.ActorNumber);
            PhotonNetwork.Instantiate(hitEffect.name, palm.position, Quaternion.identity);
            zombieControl.instance.damageBox.SetActive(false);
        
        }

        if (other.gameObject.tag == "Zombie" && damageZombie)
        {
            other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, hitDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    public void DealDamage(string damageDealer, int damageAmount, int actor)
    {
        ThirdPersonCameraControl.instance.TakeDamage(damageDealer, damageAmount, actor);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
