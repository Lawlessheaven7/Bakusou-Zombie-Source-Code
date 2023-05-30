using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class PhotonStatus : MonoBehaviourPun
{
	// Start is called before the first frame update
	private readonly string connectionStatusMessage = "Connection Status: ";

	[Header("UI References")]
	public TMP_Text ConnectionStatusText;

	public void Update()
	{
		if (PhotonNetwork.IsConnected)
		{
			ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
		}
	}
}
