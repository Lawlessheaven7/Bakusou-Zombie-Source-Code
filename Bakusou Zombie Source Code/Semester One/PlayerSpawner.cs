using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPun
{

    public static PlayerSpawner instance;

    private void Awake()
    {
        instance = this;

    }

    public GameObject playerPrefab;
    public GameObject zombiePrefab;
    public GameObject deathEffect;
    public GameObject player;
    public GameObject zombies;
    public GameObject deathposition;
    public bool zombie = false;

    public float respawnTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void Update()
    {
        if(player != null)
        {
            deathposition.transform.position = player.transform.position;
        }
        
    }

    public void SpawnPlayer()
    {

        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        //UpdateLeaderBoard.instance.UpdateBoard();
    }

 

    public void SpawnZombie()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        zombies = PhotonNetwork.Instantiate(zombiePrefab.name, spawnPoint.position, spawnPoint.rotation);
        //UpdateLeaderBoard.instance.UpdateBoard();

    }

    public void SpawnZombie2()
    {
        Transform spawnPoint = deathposition.transform;

        zombies = PhotonNetwork.Instantiate(zombiePrefab.name, spawnPoint.position, spawnPoint.rotation);
        //UpdateLeaderBoard.instance.UpdateBoard();

    }


    //Respawn players after death
    public void Death(string damageDealer)
    {
        
        if(player != null )
        {
            StartCoroutine(DiecoP());
        }

        if(zombies != null)
        {
            StartCoroutine(DiecoZ());
        }

        MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        
    }


    public void eventZombie()
    {
        //PhotonNetwork.Destroy(player);
        if (player != null)
        {
            StartCoroutine(DiecoEvent());
        }
    }

    public IEnumerator DiecoP()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.Euler(-90, 0, 0));
        PhotonNetwork.Destroy(player);
        player = null;

        UIController.instance.image.SetActive(false);
        UIController.instance.deathScreen.SetActive(true);
        UIController.instance.deathText.SetActive(true);

        //set respawn time
        yield return new WaitForSeconds(respawnTime);

        UIController.instance.deathScreen.SetActive(false);
        UIController.instance.deathText.SetActive(false);
   

        if(MatchManager.instance.state == MatchManager.Gamestate.Playing && player == null)
        {
            SpawnZombie();
        }
    }

    public IEnumerator DiecoZ()
    {
        PhotonNetwork.Instantiate(deathEffect.name, zombies.transform.position, Quaternion.Euler(-90, 0, 0));
        PhotonNetwork.Destroy(zombies);
        zombies = null;

        UIController.instance.image.SetActive(false);
        UIController.instance.deathScreen.SetActive(true);
        UIController.instance.deathText.SetActive(true);

        //set respawn time
        yield return new WaitForSeconds(respawnTime);

        UIController.instance.deathScreen.SetActive(false);
        UIController.instance.deathText.SetActive(false);


        if (MatchManager.instance.state == MatchManager.Gamestate.Playing && player == null)
        {
            SpawnZombie();
        }
    }

    public IEnumerator DiecoEvent()
    {
  
            PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.Euler(-90, 0, 0));
            PhotonNetwork.Destroy(player);
            player = null;
            zombie = true;
        
            //UIController.instance.image.SetActive(false);
            //UIController.instance.deathScreen.SetActive(true);
            //UIController.instance.deathText.SetActive(true);

            //set respawn time
            yield return new WaitForSeconds(0);

            UIController.instance.deathScreen.SetActive(false);
            UIController.instance.deathText.SetActive(false);


            if (MatchManager.instance.state == MatchManager.Gamestate.Playing && player == null && zombie == true)
            {
            SpawnZombie2();
            }
       
    }



}
