using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }
    [Header("Room and CachedRoom")]
    public List<CachedRoom> cacheRoomList;
    public List<RoomEntry> roomEntryList;

    [Header("Room List Panel")]
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    CachedRoom TmpCache;

    public GameObject loadingScreen;
    public GameObject menuButtons;
    public GameObject createRoomScreen;
    public GameObject roomScreen;
    public GameObject errorScreen;
    public GameObject roomBrowserScreen;
    public GameObject nameInputScreen;
    public GameObject startButton;
    public GameObject startingSoon;
    public GameObject optionScreen;
    public GameObject refreshButton;
    public GameObject chat;
    public GameObject CreditScreen;
    public GameObject emergencyOptions;
    public GameObject matchStartWarning;
    public GameObject leaveButton;

    public TMP_Text loadingText, nbrplayerText;
    public TMP_Text roomText, playerNameLabel;
    public TMP_Text errorText;
    public TMP_Text countdownDisplay;

    public TMP_InputField roomNameInput;
    public TMP_InputField NameInput;

    public RoomButton theRoomButton;

    public AudioSource countDown;

    private List<RoomButton> allRoomButtons = new List<RoomButton>();
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    private int nbrPlayersInLobby = 0;
    public int countdonwTime;

    public static bool hasSetUsername;

    public string levelToPlay;

    public PhotonView myPV;

    [Header("All Maps")]
    public string[] allMaps;
    public bool changeMapBetweenRounds = true;


    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        CloseMenus();
        roomNameInput.characterLimit = 20;
        NameInput.characterLimit = 16;

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network . . . ";
        nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;


        //Connect to the Photon Server using our custom settings.
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
//Only display the test button in the Editor version. 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    //Function to control the UI element, called in the Start() to have all the unecessary UI set to false.
    void CloseMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
        optionScreen.SetActive(false);
        chat.SetActive(false);
        CreditScreen.SetActive(false);
       
    }

    //Action that happens after connecting to the master server
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby(); //Once connected to the master immediately join the Lobby.
        nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;
        nbrplayerText.text = "Online: " + nbrPlayersInLobby.ToString("00");
        PhotonNetwork.AutomaticallySyncScene = true;

        loadingText.text = "Joining Lobby . . .";
    }

    //Action that happens after joining the lobby
    public override void OnJoinedLobby()
    {
        CloseMenus();
        refreshButton.SetActive(true);
        menuButtons.SetActive(true);

        if (!hasSetUsername)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);

            //If there are something stored in the playername, then set the username to the playername value
            if (PlayerPrefs.HasKey("playerName"))
            {
                NameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
        
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
    }

    //Function to create a room in the lobby.
    public void CreateRoom()
    {
        //if the room name input field is not empty, players are allowed to create a room with the option settings in the scripts.
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 15;
            


            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            CloseMenus();
            loadingText.text = "Creating Room . . . ";
            loadingScreen.SetActive(true);
        }
    }

    //Actions that happened once player joins the room
    public override void OnJoinedRoom()
    {
        refreshButton.SetActive(false);
        CloseMenus();
        roomScreen.SetActive(true);
        chat.SetActive(true);
  


        roomText.text = PhotonNetwork.CurrentRoom.Name;
    

        ListAllPlayers();

        //Check if the current player is the master, if not then the start game button will not show.
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            startingSoon.SetActive(false);
        }
        else
        {
            startButton.SetActive(false);
            startingSoon.SetActive(true);
        }
        //Chat.instance.SendMsgConnection(PhotonNetwork.LocalPlayer.NickName);
    }

    //List all the players, get the information from the network and store it for display.
    private void ListAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;

        for(int i = 0; i< players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }


    }

    //When a player joins the room display their name
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    
    }

    //Refresh and list all current players
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
   
    }

    // WHen creating the room failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed To Create Room: " + message;
        CloseMenus();
        errorScreen.SetActive(true);
    }

    //Error screen utility function to reopen the menu
    public void closeErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    //Leaving the room
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room . . . ";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
        Chat.instance.TextChat.text = "";
        
    }

    //Open the room browser
    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }

    //Close the room browser
    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    //Called at anytime when there is a change of the room while in the lobby.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);

        foreach (RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        //Loop through all the roomlist information, then regulate the rooms.
        for(int i = 0; i < roomList.Count; i++)
        {
            //As long as the players aren't full and it hasn't been removed from the list, then display the room.
            if(roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                foreach (CachedRoom info in cacheRoomList)
                {
                    RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                    newButton.SetButtonDetails(roomList[i]);
                    newButton.GetComponent<RoomButton>().Init(info.countplayer, info.maxplayer);
                    newButton.gameObject.SetActive(true);

                    allRoomButtons.Add(newButton);
                    RoomEntry roomEntry = new RoomEntry();
                    roomEntry.name = info.name;
                    roomEntry.obj = newButton.gameObject;
                    roomEntryList.Add(roomEntry);
                }
            }
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {

            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cacheRoomList.Contains(TmpCache))
                {
                    cacheRoomList.Remove(TmpCache);
                }
                continue;
            }

            foreach (CachedRoom im in cacheRoomList)
            {
                if (info.Name == im.name)
                {
                    TmpCache = im;
                }
            }

            // Update cached room info
            if (cacheRoomList.Contains(TmpCache))
            {
                TmpCache.rInfo = info;
            }
            // Add new room info to cache
            else
            {
                CachedRoom cacheRoom = new CachedRoom();
                cacheRoom.name = info.Name;
                cacheRoom.countplayer = info.PlayerCount;
                cacheRoom.maxplayer = info.MaxPlayers;
                cacheRoom.rInfo = info;
                cacheRoomList.Add(cacheRoom);
            }
        }
    }

    // Tell Photon Network to let player join the room after clicking the room name
    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseMenus();
        loadingText.text = "Joining Room ";
        loadingScreen.SetActive(true);
    }

    //set player username
    public void setUsername()
    {
        if (!string.IsNullOrEmpty(NameInput.text))
        {
            PhotonNetwork.NickName = NameInput.text;
            //save the username into a string
            PlayerPrefs.SetString("playerName", NameInput.text);

            CloseMenus();
            menuButtons.SetActive(true);

            hasSetUsername = true;
        }
    }
    public void openOptions()
    {
        CloseMenus();
        optionScreen.SetActive(true);
    }

    public void closeOptions()
    {
        menuButtons.SetActive(true);
        optionScreen.SetActive(false);
    }

    public void openCredits()
    {
        CloseMenus();
        CreditScreen.SetActive(true);
    }

    public void closeCredits()
    {
        menuButtons.SetActive(true);
        CreditScreen.SetActive(false);
    }

    public void openEmergency()
    {
        emergencyOptions.SetActive(true);
    }

    public void closeEmergency()
    {
        emergencyOptions.SetActive(false);
    }

    //Start the Game
    public void StartGame()
    {

        myPV.RPC("preStartGame", RpcTarget.All);

    }

    [PunRPC]
    public void preStartGame()
    {
        StartCoroutine(matchCountDown());
    }

 

    public void StartGame2()
    {
        PhotonNetwork.LoadLevel(2);
    }

    //If game master leaves the room and the master is switched to another player, shows the start game button
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
            startingSoon.SetActive(false);
        }
        else
        {
            startButton.SetActive(false);
            startingSoon.SetActive(true);
        }
    }

    public void OnRefreshButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
        nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;
        nbrplayerText.text = "Online: " + nbrPlayersInLobby.ToString("00");
        PhotonNetwork.JoinLobby();
    }

    public void QuickTest()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;

        PhotonNetwork.CreateRoom("Test", options);
        CloseMenus();
        loadingText.text = "Creating Room";
        loadingScreen.SetActive(true);
    }

    public void ClearRoomListView()
    {
        foreach (RoomEntry entry in roomEntryList)
        {
            Destroy(entry.obj);
        }
        roomEntryList.Clear();
        cacheRoomList.Clear();
    }

    //Close the build Version of the game, does nothing to the editor version
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator matchCountDown()
    {
        matchStartWarning.SetActive(true);
        countDown.Play();
        leaveButton.SetActive(false);
        startButton.SetActive(false);
        
        while (countdonwTime > 0)
        {
            Debug.Log("Played");
            countdownDisplay.text = countdonwTime.ToString();
            yield return new WaitForSeconds(1f);
            countdonwTime--;
        }

        //yield return new WaitForSeconds(1);
        matchStartWarning.SetActive(false);
        PhotonNetwork.LoadLevel(1);
    }

    [System.Serializable]
    public struct CachedRoom
    {
        public string name;
        public int countplayer;
        public int maxplayer;
        public RoomInfo rInfo;
    }

    [System.Serializable]
    public struct RoomEntry
    {
        public string name;
        public GameObject obj;
    }

    

}