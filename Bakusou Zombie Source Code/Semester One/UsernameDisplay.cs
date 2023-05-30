using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class UsernameDisplay : MonoBehaviour
{
    public static UsernameDisplay instance;
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMP_Text username;
    [SerializeField] GameObject NameTag;
   
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        if (playerPV.IsMine)
        {
            NameTag.gameObject.SetActive(false);
           
        }

        username.text = playerPV.Owner.NickName;
    }

}
