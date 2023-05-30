using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickupSpawner : MonoBehaviourPun
{
    public static PickupSpawner instance;
    //public float NumbersOfPickUp;
    public GameObject[] weaponPikcUp;
    public GameObject weaponPrefab;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        weaponPikcUp = GameObject.FindGameObjectsWithTag("WeaponPickUp");


    }

    public void spawnWeaponPickUp()
    {
        /*if (NumbersOfPickUp > weaponPikcUp.Length)
        {
            Transform spawnPoint = pickUpSpawnPointManager.instance.GetSpawnPoint();
            PhotonNetwork.Instantiate(weaponPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }*/
        
    }



   
}
