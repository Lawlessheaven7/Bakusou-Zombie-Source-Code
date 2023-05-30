using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunPlayerTag : MonoBehaviourPunCallbacks
{
    public static PunPlayerTag Instance;
    //needed for player properties
    public const string TagKey = "tag";

    private void Awake()
    {
        Instance = this;
    }
    public static void AssignTagToRandomPlayer(string tag)
    {
        var players = PhotonNetwork.PlayerList;
        Debug.Log("players: " + players.Length);
        var randomPlayer = players[Random.Range(0, players.Length)];
        SetTag(randomPlayer, tag);
    }

    public static Player GetPlayerWithTag(string tag)
    {
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (string.Equals(GetTag(player), tag))
            {
                return player;
            }
        }

        return null;
    }

    public static List<Player> GetPlayersWithTag(string tag)
    {
        List<Player> playersWithTag = new List<Player>();
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (string.Equals(GetTag(player), tag))
            {
                playersWithTag.Add(player);
            }
        }

        return playersWithTag;
    }

    private static void SetTag(Player player, string tag)
    {
        var customProp = new Hashtable() { { PunPlayerTag.TagKey, tag } };
        player.SetCustomProperties(customProp);
    }

    private static string GetTag(Player player)
    {
        if (player.CustomProperties.TryGetValue(PunPlayerTag.TagKey, out var value))
        {
            return (string)value;
        }

        return "";
    }
}

