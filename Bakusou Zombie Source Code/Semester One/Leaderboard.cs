using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    
    public TMP_Text Survivors, killsText, Zombies;

    private void Awake()
    {
      
    }

    private void Update()
    {
        Zombies.text = MatchManager.instance.Zombies.Length.ToString() + ": " + "Zombies";
        Survivors.text = "Survivors: " + (MatchManager.instance.playersnbr - MatchManager.instance.Zombies.Length).ToString();
    }

    public void UpdateLeaderBoard()
    {
    
    }
}
