using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateLeaderBoard : MonoBehaviour
{
    // Start is called before the first frame update
    public static UpdateLeaderBoard instance;
    public TMP_Text Survivors, killsText, Zombies;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {

    }

    public void UpdateBoard()
    {
        Zombies.text = MatchManager.instance.Zombies.Length.ToString() + ": " + "Zombies";
        Survivors.text = "Survivors: " + (MatchManager.instance.playersnbr - MatchManager.instance.Zombies.Length).ToString();
    }
}

