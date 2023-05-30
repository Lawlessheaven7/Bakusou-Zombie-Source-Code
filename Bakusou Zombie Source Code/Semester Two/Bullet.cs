using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float moveSpeed, lifeTime;

    private Rigidbody bulletRigibody;

    public GameObject impactEffect;

    public int shotDamage;

    public bool damagePlayer, damageZombie;

    [Header("Special Ability")]
    public bool ice;

    public bool execution;

    public bool fire;
    private void Awake()
    {
        bulletRigibody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
       
        bulletRigibody.velocity = transform.forward * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.tag == "Player" && damagePlayer)
        {
            other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        if (other.gameObject.tag == "Zombie" && damageZombie)
        {
            other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
            if (ice)
            {
                other.gameObject.GetPhotonView().RPC("iceSlow", RpcTarget.All);
            }

            if (execution)
            {
                other.gameObject.GetPhotonView().RPC("execution", RpcTarget.All);
            }

            if (fire)
            {
                other.gameObject.GetPhotonView().RPC("onFire", RpcTarget.All);
            }
        }

        Destroy(gameObject);
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate(impactEffect.name, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
        }
    }

    [PunRPC]
    public void DealDamage(string damageDealer, int damageAmount, int actor)
    {
        ThirdPersonCameraControl.instance.TakeDamage(damageDealer, damageAmount, actor);
    }

    /*public void TakeDamage(string damageDealer, int damageAmount, int actor)
    {
        if (photonView.IsMine)
        {
            //Debug.Log(photonView.Owner.NickName + "I've been hit by" + damageDealer);

            ThirdPersonCameraControl.instance.currentHealth -= damageAmount;

            if (ThirdPersonCameraControl.instance.currentHealth <= 0)
            {
                ThirdPersonCameraControl.instance.currentHealth = 0;

                PlayerSpawner.instance.Death(damageDealer);

                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
            }

            UIController.instance.healthBar.value = ThirdPersonCameraControl.instance.currentHealth;

        }

    }*/
}
