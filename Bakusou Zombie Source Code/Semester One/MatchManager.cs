using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using UnityEngine.SocialPlatforms.Impl;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;



    private void Awake()
    {
        instance = this;
    }

    //Events
    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat,
        NextMatch,
        TimerSync
    }

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;
    //private List<Leaderboard> leaderboardPlayers = new List<Leaderboard>();

    public GameObject[] Zombies;
    public int playersnbr;
    //public int zombieindex;

    //end match
    public enum Gamestate
    {
        Waiting,
        Playing,
        Ending,
    }

    //public int killsTowin;
    public Transform mapCamPoint;
    public Gamestate state = Gamestate.Waiting;
    public float waitAfterEnding = 5f;
    public bool perpetual;
    public bool spawnZombie = false;

    public float matchLength = 190f;
    public float EventTime;
    public float EventTriggerTime = 30;
    private float currentMatchTime;
    private float sendTimer;
    PhotonView myPV;

    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        EventTime = matchLength - EventTriggerTime;
        //UpdateLeaderBoard.instance.UpdateBoard();
        mapCamPoint.gameObject.SetActive(false);
        //If player is not connected load the Main Menu
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);

            state = Gamestate.Playing;

            SetupTimer();
        }

        SetupTimer();
        /*if (PhotonNetwork.IsMasterClient)
        {
            PickZombie();
        }*/

        if (!PhotonNetwork.IsMasterClient)
        {
            UIController.instance.TimerText.gameObject.SetActive(false);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            myPV.RPC("MakePlayerZombie_RPC", PhotonNetwork.MasterClient);
            int nextMaster = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[nextMaster]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (currentMatchTime <= EventTime)
        {
            spawnZombie = true;
        }

        if (state == Gamestate.Playing && currentMatchTime <= EventTime)
        {
            spawnZombie = true;

            if (PhotonNetwork.IsMasterClient)
            {
                //PlayerSpawner.instance.eventZombie();
                //int zombie = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
                //Debug.Log(zombie);
                if (Zombies.Length == 0)
                {
                    //int zombie = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
                    myPV.RPC("MakePlayerZombie_RPC", PhotonNetwork.MasterClient);
                }
            }
           
        }

 

        if (Input.GetKeyDown(KeyCode.Tab) && state != Gamestate.Ending)
        {
            if (UIController.instance.leaderBoard.activeInHierarchy)
            {
                UIController.instance.leaderBoard.SetActive(false);
            }
            else
            {
                showLeaderBoard();
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (currentMatchTime >= 0f && state == Gamestate.Playing)
            {
                currentMatchTime -= Time.deltaTime;

                if (currentMatchTime <= 0)
                {
                    currentMatchTime = 0f;

                    state = Gamestate.Ending;

                    ListPlayerSend();

                    //StateCheck();
                    
                }

                updateTimer();

                sendTimer -= Time.deltaTime;

                if(sendTimer <= 0)
                {
                    sendTimer += 1f;

                    Timersend();
                }
            }
        }

        Zombies = GameObject.FindGameObjectsWithTag("Zombie");
        playersnbr = PhotonNetwork.CurrentRoom.PlayerCount;

        ScoreCheck();






    }

    [PunRPC]
    void MakePlayerZombie_RPC()
    {
        ThirdPersonCameraControl.instance.isInfected = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        //Photon has 255 codes, but anything above 200 is reserved to Photon.
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            //Debug.Log("Received Event" + theEvent);

            switch (theEvent)
            {
                case EventCodes.NewPlayer:

                    NewPlayerReceive(data);

                    break;

                case EventCodes.ListPlayers:

                    ListPlayerReceive(data);

                    break;

                case EventCodes.UpdateStat:

                    UpdateStatsReceive(data);

                    break;

                case EventCodes.NextMatch:

                    NextMatchReceived();

                    break;

                case EventCodes.TimerSync:

                    TimerReceived(data);

                    break;
            }
        }
    }

    public override void OnEnable()
    {
        //Add to the list so when event is declared the list will follow
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //sending player information 
    public void NewPlayerSend(string username)
    {
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;

        PhotonNetwork.RaiseEvent(
        (byte)EventCodes.NewPlayer,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
        new SendOptions { Reliability = true }
        );
    }

    public void NewPlayerReceive(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);

        allPlayers.Add(player);

        ListPlayerSend();
    }

    public void ListPlayerSend()
    {
        object[] package = new object[allPlayers.Count + 1];

        package[0] = state;

        //Loop to collect all the player Info
        for(int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[4];

            piece[0] = allPlayers[i].name;

            piece[1] = allPlayers[i].actor;

            piece[2] = allPlayers[i].kills;

            piece[3] = allPlayers[i].deaths;

            package[i + 1] = piece;
        }

       //Send the event
       PhotonNetwork.RaiseEvent(
       (byte)EventCodes.ListPlayers,
       package,
       new RaiseEventOptions { Receivers = ReceiverGroup.All },
       new SendOptions { Reliability = true }
       );
    }

    public void ListPlayerReceive(object[] dataReceived)
    {
        allPlayers.Clear();

        state = (Gamestate)dataReceived[0];

        for(int i = 1; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
                );

            allPlayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }
        }

        StateCheck();
    }

    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

       PhotonNetwork.RaiseEvent(
       (byte)EventCodes.UpdateStat,
       package,
       new RaiseEventOptions { Receivers = ReceiverGroup.All },
       new SendOptions { Reliability = true }
       );
    }

    public void UpdateStatsReceive(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        for(int i = 0; i < allPlayers.Count; i++)
        {
            if(allPlayers[i].actor == actor)
            {
                switch (statType)
                {

                    case 0://kills
                        allPlayers[i].kills += amount;
                        Debug.Log("player" + allPlayers[i].name + ": kills " + allPlayers[i].kills);
                        break;
                    case 1://Deaths
                        allPlayers[i].deaths += amount;
                        Debug.Log("player" + allPlayers[i].name + ": deaths " + allPlayers[i].deaths);
                        break;

                }

                if(i == index)
                {
                    UpdateStatsDisplay();
                }

                if (UIController.instance.leaderBoard.activeInHierarchy)
                {
                    showLeaderBoard();
                }

                break;
            }

        }

        ScoreCheck();
    }

    public void UpdateStatsDisplay()
    {
        if(allPlayers.Count > index)
        {

     
        }
        else
        {
  
        }

    }

    void showLeaderBoard()
    {
        UIController.instance.leaderBoard.SetActive(true);

        /*foreach(Leaderboard lp in leaderboardPlayers)
        {
            Destroy(lp.gameObject);
        }
        leaderboardPlayers.Clear();

        UIController.instance.leaderboardPlayerDisplay.gameObject.SetActive(false);

        List<PlayerInfo> sorted = SortPlayers(allPlayers);

        foreach(PlayerInfo player in sorted)
        {
            Leaderboard newPlayerDisplay = Instantiate(UIController.instance.leaderboardPlayerDisplay, UIController.instance.leaderboardPlayerDisplay.transform.parent);

            newPlayerDisplay.SetDetails(player.name, player.kills, player.deaths);

            newPlayerDisplay.gameObject.SetActive(true);

            leaderboardPlayers.Add(newPlayerDisplay);
        }*/
    }

    private List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();

        while(sorted.Count < players.Count)
        {
            int highest = -1;
            PlayerInfo selectedPlayer = players[0];

            foreach(PlayerInfo player in players)
            {
                if (!sorted.Contains(player))
                {
                    if (player.kills > highest)
                    {
                        selectedPlayer = player;
                        highest = player.kills;
                    }
                }

            }

            sorted.Add(selectedPlayer);
        }

        return sorted;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(0);
    }

    public void ScoreCheck()
    {
        bool winnerFound = false;

        if(Zombies.Length == playersnbr)
        {
           winnerFound = true;
        }

        foreach(PlayerInfo player in allPlayers)
        {
            /*if(player.kills >= killsTowin && killsTowin > 0)
            {
                winnerFound = true;

                break;
            }*/
        }

        if (winnerFound)
        {
            if(PhotonNetwork.IsMasterClient && state != Gamestate.Ending)
            {
                state = Gamestate.Ending;
                ListPlayerSend();
            }
        }
    }

    void StateCheck()
    {
        if(state == Gamestate.Ending)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        state = Gamestate.Ending;
        mapCamPoint.gameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        UIController.instance.endScreen.SetActive(true);
        //showLeaderBoard();
        if(currentMatchTime <= 0)
        {
            UIController.instance.SurvivorWins.SetActive(true);
        } else if(currentMatchTime > 0)
        {
            UIController.instance.ZombieWins.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Cursor.visible = true;
        ThirdPersonCameraControl.instance.isInfected = false; 
        StartCoroutine(endCo());


    }

    private IEnumerator endCo()
    {
        yield return new WaitForSeconds(waitAfterEnding);

        if (!perpetual)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {

                if (!Launcher.instance.changeMapBetweenRounds)
                {
                    NextMatchSend();
                }
                else
                {
                    int newLevel = Random.Range(0, Launcher.instance.allMaps.Length);

                    if (Launcher.instance.allMaps[newLevel] == SceneManager.GetActiveScene().name)
                    {
                        NextMatchSend();
                    }
                    else
                    {
                        PhotonNetwork.LoadLevel(Launcher.instance.allMaps[newLevel]);
                    }
                }
            }
        }
   
    }

    public void NextMatchSend()
    {
       PhotonNetwork.RaiseEvent(
       (byte)EventCodes.NextMatch,
       null,
       new RaiseEventOptions { Receivers = ReceiverGroup.All },
       new SendOptions { Reliability = true }
       );
    }

    public void NextMatchReceived()
    {
        state = Gamestate.Playing;
        spawnZombie = false;
        UIController.instance.leaderBoard.SetActive(false);
        UIController.instance.ZombieWins.SetActive(false);
        UIController.instance.SurvivorWins.SetActive(false);
        UIController.instance.endScreen.SetActive(false);


        foreach (PlayerInfo player in allPlayers)
        {
            player.kills = 0;
            player.deaths = 0;
        }

        UpdateStatsDisplay();
        //UpdateLeaderBoard.instance.UpdateBoard();

        if (PlayerSpawner.instance.player == null)
        {
            PlayerSpawner.instance.SpawnPlayer();
        }

        SetupTimer();
        mapCamPoint.gameObject.SetActive(false);

       

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetupTimer()
    {
        if(matchLength > 0)
        {
            currentMatchTime = matchLength;
            updateTimer();
        }
    }

    public void updateTimer()
    {
        var timeToDisplay = System.TimeSpan.FromSeconds(currentMatchTime);

        UIController.instance.TimerText.text = timeToDisplay.Minutes.ToString("00") + " : " + timeToDisplay.Seconds.ToString("00");
    }

    public void Timersend()
    {
        object[] package = new object[] { (int)currentMatchTime, state };

        PhotonNetwork.RaiseEvent(
        (byte)EventCodes.TimerSync,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
);
    }

    public void TimerReceived(object[] dataReceived)
    {
        currentMatchTime = (int) dataReceived[0];
        state = (Gamestate)dataReceived[1];

        updateTimer();

        UIController.instance.TimerText.gameObject.SetActive(true);

    }

}



[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, deaths;

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }

}
