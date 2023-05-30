using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickUp : MonoBehaviour
{
    public PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        pv = pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("Heal", RpcTarget.All);

                Debug.Log("Heal");
            }

            pv.RPC("active", RpcTarget.All);
        }

        if(other.tag == "Zombie")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("DamageAura", RpcTarget.All);
                Debug.Log("Area Damage Activated");
            }
            pv.RPC("active", RpcTarget.All);
        }
    }

    private void EnableAfterCooldown()
    {
        Transform spawnPoint = pickUpSpawnPointManager.instance.GetSpawnPoint2();
        gameObject.SetActive(true);
        gameObject.transform.position = spawnPoint.position;
    }

    [PunRPC]
    public void active()
    {
        gameObject.SetActive(false);
        Invoke("EnableAfterCooldown", 30f);
    }
}
