using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public TMP_Text buttonText, sloganText;

    private RoomInfo info;


    //Store information to display room name
    public void SetButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;

        buttonText.text = info.Name;

        
    }

    public void Init(int CountPlayer, int MaxPlayer)
    {
        sloganText.text = "Players: " + CountPlayer.ToString("00") + "/" + MaxPlayer.ToString("00");
    }

    public void OpenRoom()
    {
        Launcher.instance.JoinRoom(info);
    }

}
