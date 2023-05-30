using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using TMPro;

namespace Photon.Pun
{
    public class Chat : MonoBehaviour
    {
		public static Chat instance;
        public ScrollRect myScrollRect;
        public TMP_InputField TextSend;
        public TMP_Text TextChat;
        public GameObject TextSendObj;
        public GameObject EmoteFrames;
        public string[] ShortcutEmotes;
        public bool isSelect = false;
        private PhotonView photonView;

        private void Awake()
        {
			instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            photonView = GetComponent<PhotonView>();
        }

        // Update is called once per frame
        void Update()
        {
			if (Input.GetKeyDown(KeyCode.Return) && !isSelect)
			{
				//EventSystem.current.SetSelectedGameObject (TextSendObj.gameObject, null);
				EventSystem.current.SetSelectedGameObject(TextSend.gameObject, null);
				isSelect = true;
			}
			else if (Input.GetKeyDown(KeyCode.Return) && isSelect && TextSend.text.Length > 0)
			{
				isSelect = false;
				string msg = TextSend.text;
				EventSystem.current.SetSelectedGameObject(null);
				sendChatOfMaster(msg);
				TextSend.text = "";
			}
			else if (Input.GetKeyDown(KeyCode.Return) && isSelect && TextSend.text.Length == 0)
			{
				isSelect = false;
				//TextSendObj.SetActive (false);
				EventSystem.current.SetSelectedGameObject(null);
				TextSend.text = "";
			}
			else if (!isSelect && TextSend.text.Length > 0)
			{
				isSelect = true;
				EventSystem.current.SetSelectedGameObject(TextSend.gameObject, null);
			}

		}

		public void sendChatOfMaster(string msg)
		{
			if (msg != "")
			{
				bool isMaster = false;
				if (PhotonNetwork.IsMasterClient)
				{
					isMaster = true;
				}
				photonView.RPC("sendChatMaster", RpcTarget.MasterClient, isMaster, msg, PhotonNetwork.LocalPlayer.NickName);
				TextSend.text = "";
			}
		}

		public void sendChatOfMasterViaBtn()
		{
			string msg = TextSend.text;
			if (msg != "")
			{
				bool isMaster = false;
				if (PhotonNetwork.IsMasterClient)
				{
					isMaster = true;
				}
				photonView.RPC("sendChatMaster", RpcTarget.MasterClient, isMaster, msg, PhotonNetwork.LocalPlayer.NickName);
				TextSend.text = "";
			}
		}

		public void ShowEmotes()
		{
			if (EmoteFrames.activeSelf)
			{
				EmoteFrames.SetActive(false);
			}
			else
			{
				EmoteFrames.SetActive(true);
			}
		}

		public void AddEmotes(int idSmiley)
		{
			TextSend.text += " " + ShortcutEmotes[idSmiley];
			EmoteFrames.SetActive(false);
		}


		[PunRPC]
		public void sendChatMaster(bool master, string msg, string pse)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				photonView.RPC("SendMsg", RpcTarget.All, master, msg, pse);
			}
		}

		[PunRPC]
		public void SendMsg(bool master, string msg, string pse)
		{
			for (int i = 0; i < ShortcutEmotes.Length; i++)
			{
				msg = msg.Replace(ShortcutEmotes[i], " <size=150%><sprite=" + i + "><size=100%>");
			}

			if (master)
			{
				TextChat.text += "<color=#a52a2aff>" + pse + " : </color><color=#ffffffff>" + msg + "</color>\n";
			}
			else
			{
				TextChat.text += "<color=#add8e6ff>" + pse + " : </color><color=#ffffffff>" + msg + "</color>\n";
			}
			myScrollRect.verticalNormalizedPosition = 0f;
		}

		public void OnPhotonPlayerConnected(Player player)
		{
			SendMsgConnectionMaster(player.NickName);
		}

		public void SendMsgConnection(string pse)
		{
			photonView.RPC("SendMsgConnectionMaster", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
		}

		[PunRPC]
		public void SendMsgConnectionMaster(string pse)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				photonView.RPC("SendMsgConnectionAll", RpcTarget.All, pse);
			}
		}

		[PunRPC]
		public void SendMsgConnectionAll(string pse)
		{
			TextChat.text += "<color=#ffa500ff><i>New connection </i></color><color=#add8e6ff><i>" + pse + "</i></color>\n";
		}

		public void SelectInputByClick()
		{
			if (!isSelect)
			{
				isSelect = true;
			}
		}
	}
}
