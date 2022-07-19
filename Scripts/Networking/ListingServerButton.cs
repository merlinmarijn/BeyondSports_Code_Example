using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ListingServerButton : MonoBehaviourPunCallbacks
{
	public TextMeshProUGUI Server_Name_Text;
	public TextMeshProUGUI Server_Mode;
	public TextMeshProUGUI Server_Max_Players;
	public TextMeshProUGUI Server_Map;

	public string Server_Name;

	public RoomInfo RoomInfo
	{
		get;
		private set;
	}

	public void SetRoomInfo(RoomInfo roomInfo)
	{
        RoomInfo = roomInfo;
		Server_Name_Text.text = (string)roomInfo.CustomProperties["name"];
		int enumVal;
		int.TryParse(roomInfo.CustomProperties["mode"].ToString(), out enumVal);
		Server_Mode.text = ((GamemodeSingleton.Gamemodes)enumVal).ToString();
		Server_Max_Players.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
		int.TryParse(roomInfo.CustomProperties["map"].ToString(), out enumVal);
		Server_Map.text = ((GamemodeSingleton.Maps)enumVal).ToString();
		//Server_Name = roomInfo.Name;
	}

	public void JoinRoom()
	{
		LobbyNetworkManager component = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<LobbyNetworkManager>();
		component.JoinRoom(RoomInfo);
	}
}
