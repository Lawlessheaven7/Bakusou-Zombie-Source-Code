using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class pickZombie : MonoBehaviourPunCallbacks, IPunObservable
{
    public static pickZombie localPlayer;
    public bool isZombie;
    PhotonView myPV;
    // Start is called before the first frame update

    private void Awake()
    {
        myPV = GetComponent<PhotonView>();

        if (myPV.IsMine)
        {
            localPlayer = this;
        }
    }
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isZombie);
        }
        else
        {
            this.isZombie = (bool)stream.ReceiveNext();
        }
    }


    public void BecomeZombie(int zombie)
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[zombie])
        {
            isZombie = true;
            Debug.Log("true");
        }
    }
}
