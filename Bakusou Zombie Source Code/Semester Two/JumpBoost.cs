using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class JumpBoost : MonoBehaviourPun
{
    public static JumpBoost instance;

    public GameObject playerJumpVFX;
    public Transform vfxSpawnPosition;
    public bool triggerentered;
    public bool player;
    public bool zombie;
    public bool megaBoost;

    public PhotonView pv;
    


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //currentBoostHeight = boostHeight;

        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerentered && Input.GetKeyDown(KeyCode.Space))
        {
            if (player)
            {
                PhotonNetwork.Instantiate(playerJumpVFX.name, PlayerSpawner.instance.player.transform.position, Quaternion.identity);
            }
            else if(zombie)
            {
                PhotonNetwork.Instantiate(playerJumpVFX.name, PlayerSpawner.instance.zombies.transform.position, Quaternion.identity);
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
       
      
        if (other.gameObject.tag == "Player")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                //ThirdPersonController.instance.boostAmount = currentBoostHeight;
                triggerentered = true;
                if (!megaBoost)
                {
                    photonView.RPC("jumpBoost", RpcTarget.All);
                }
                else
                {
                    photonView.RPC("megaBoost", RpcTarget.All);
                }
                player = true;
                zombie = false;

            }
        }

        if (other.gameObject.tag == "Zombie")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                //ThirdPersonController.instance.boostAmount = currentBoostHeight;
                triggerentered = true;
                if (!megaBoost)
                {
                    photonView.RPC("jumpBoost", RpcTarget.All);
                }
                else
                {
                    photonView.RPC("megaBoost", RpcTarget.All);
                }
                zombie = true;
                player = false;

            }
        }



    }
    private void OnTriggerExit(Collider other)
    {
       
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Zombie")
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                triggerentered = false;
                photonView.RPC("endBoost", RpcTarget.All);

            }
        }

    }


}
