using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillLayer : MonoBehaviour
{
    private int layerDamage = 200;
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
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetPhotonView().RPC("KillLayerDamage", RpcTarget.All, layerDamage);
        }

        if (other.gameObject.tag == "Zombie")
        {
            other.gameObject.GetPhotonView().RPC("killLayerDamageZombie", RpcTarget.All, layerDamage);
        }
    }

    [PunRPC]
    public void KillLayerDamage(int damageAmount)
    {
        ThirdPersonCameraControl.instance.KillLayer(damageAmount);
    }

    [PunRPC]
    public void killLayerDamageZombie(int damageAmount)
    {
        zombieControl.instance.KillLayer(damageAmount);
    }
}
