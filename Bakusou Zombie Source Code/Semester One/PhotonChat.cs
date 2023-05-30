using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PhotonChat : MonoBehaviour, IChatClientListener
{

    public void DebugReturn(DebugLevel level, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnConnected()
    {
        Debug.Log("Connected to the Photon Chat");
    }

    public void OnDisconnected()
    {
       
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
  
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
     
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
     
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
       
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
       
    }

    private ChatClient chatClient;

    [SerializeField] PhotonView playerPV;
    [SerializeField] private string userID;

    // Start is called before the first frame update
    private void Awake()
    {
       
    }

    void Start()
    {
        chatClient = new ChatClient(this);
        ConnectToPhotonChat();
       
    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();
    }

    private void ConnectToPhotonChat()
    {
        Debug.Log("Connecting to Photon Chat");
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(userID);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userID));
    
    }
}
