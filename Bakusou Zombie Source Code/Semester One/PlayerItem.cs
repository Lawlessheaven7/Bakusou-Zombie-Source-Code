using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviourPun
{
    public static PlayerItem instance;
    public GameObject[] characterList;
    public int index;
    PhotonView PV;

    private void Awake()
    {
        instance = this;
        PV = GetComponent<PhotonView>();
        characterList = new GameObject[transform.childCount];
    }

    void Start()
    {
       
        for (int i = 0; i < transform.childCount; i++)
        {
            characterList[i] = transform.GetChild(i).gameObject;
            characterList[i].SetActive(false);
        }

        if (photonView.IsMine)
        {
            index = PlayerPrefs.GetInt("CharacterSelected");

            // Notify all remote copies of us to change their index
            //
            photonView.RPC("SetCharacterIndex", RpcTarget.OthersBuffered, index);

            // Set the index locally
            //
            SetCharacterIndex(index);
        }
    }
    [PunRPC]
    private void SetCharacterIndex(int index)
    {
        if (characterList[index])
            characterList[index].SetActive(true);
    }

    public void ToggleLeft()
    {
        characterList[index].SetActive(false);
        index--;
        if (index < 0)
            index = characterList.Length - 1;
        characterList[index].SetActive(true);
    }

    public void ToggleRight()
    {
        characterList[index].SetActive(false);
        index++;
        if (index == characterList.Length)
            index = 0;
        characterList[index].SetActive(true);
    }


    public void kaydetbuton()
    {
        PlayerPrefs.SetInt("CharacterSelected", index);
    }
}

