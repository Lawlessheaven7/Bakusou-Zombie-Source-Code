using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponPickup : MonoBehaviourPun
{
    public static WeaponPickup instance;

    public string theWeapon;

    public float respawnTime = 25f;

    //private bool collected;

    public PhotonView pv;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("AddWeapon", RpcTarget.All, theWeapon);

                Debug.Log("Weapon Acquired");

            }

            pv.RPC("active", RpcTarget.All);
           
        }

        if(other.tag == "Zombie")
        {
            pv.RPC("active", RpcTarget.All);
        }

    }

    private void EnableAfterCooldown()
    {
        Transform spawnPoint = pickUpSpawnPointManager.instance.GetSpawnPoint();
        gameObject.SetActive(true);
        gameObject.transform.position = spawnPoint.position;
    }

    [PunRPC]
    public void active()
    {
        gameObject.SetActive(false);
        Invoke("EnableAfterCooldown", respawnTime);
    }





}
