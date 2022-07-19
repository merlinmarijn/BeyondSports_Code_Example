using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
	public ScriptableRoomSettings RoomSettings;

	public List<RoomInfo> RI = new List<RoomInfo>();

	public LobbyRoomListing LRL => gameObject.GetComponent<LobbyRoomListing>();

	#region RoomSettings
	[Header("Dropdowns for game creation options")]
	public TMP_Dropdown mode;
	public TMP_Dropdown rounds;
	public TMP_Dropdown difficulty;
	public TMP_Dropdown maps;
	public TMP_Dropdown maxplayers;
	public Button CreationButton;
	#endregion

	[SerializeField]
	private bool AutoConnect;

	private bool canJoin => RoomSettings.Player_Name.Length >= 1; // Stond op 5?
	private bool ConnectedToLobby = false;

    //[SerializeField]
    public int SceneSelected = 1;

    [SerializeField]
	private TMP_InputField nicknamefield;


    //[SerializeField] private List<UnityEngine.Object> sceneAsset;
    [SerializeField] private List<string> sceneAsset;

    [SerializeField]
	private List<Sprite> SceneImages;
	[SerializeField]
	private Button MapSelectorBtn;

	public override void OnConnectedToMaster()
	{
		UnityEngine.Debug.Log("Connected to master server");
		PhotonNetwork.JoinLobby();
		base.OnConnectedToMaster();
	}

	public override void OnJoinedLobby()
	{
		UnityEngine.Debug.Log("Joined lobby");
		base.OnJoinedLobby();
		if (AutoConnect)
		{
			JoinRandomRoom();
		}
		ConnectedToLobby = true;
	}

	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel(sceneAsset[SceneSelected]);
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		SceneSelected = 5;
		CreateRoom
			(
			"Default Room",
			GamemodeSingleton.Gamemodes.Team,
			GamemodeSingleton.Rounds.Four,
			GamemodeSingleton.Difficulty.Normal,
			sceneAsset[SceneSelected],
			GamemodeSingleton.Maxplayers.sixteen
			);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
	}

	public void JoinRoom(RoomInfo info)
	{
		if (canJoin && ConnectedToLobby)
		{
			PhotonNetwork.JoinRoom(info.Name);
		}
	}

	public void EnableCreateButton()
    {
        if (mode.value != 0 && rounds.value != 0 && difficulty.value != 0 && SceneSelected != 0 && maxplayers.value != 0)
        {
			CreationButton.interactable = true;
        } else
        {
			CreationButton.interactable = false;
		}
	}

	public void CreateRoomButton() 
	{
		//Create room with current settings (and parse them from the dropdown menu's to the actual enum's)
		CreateRoom(
            RoomSettings.RoomName
            ,(GamemodeSingleton.Gamemodes)System.Enum.Parse(typeof(GamemodeSingleton.Gamemodes), mode.options[mode.value].text)
            ,(GamemodeSingleton.Rounds)System.Enum.Parse(typeof(GamemodeSingleton.Rounds), rounds.options[rounds.value].text)
            ,(GamemodeSingleton.Difficulty)System.Enum.Parse(typeof(GamemodeSingleton.Difficulty), difficulty.options[difficulty.value].text)
            , sceneAsset[SceneSelected]/*(GamemodeSingleton.Maps)System.Enum.Parse(typeof(GamemodeSingleton.Maps), maps.options[maps.value].text)*/
			, (GamemodeSingleton.Maxplayers)System.Enum.Parse(typeof(GamemodeSingleton.Maxplayers), maxplayers.options[maxplayers.value].text)
            ); 
	}

	public void CreateRoom(
		string name = "default server"
		, GamemodeSingleton.Gamemodes mode = GamemodeSingleton.Gamemodes.Solo
		, GamemodeSingleton.Rounds rounds = GamemodeSingleton.Rounds.Two
		, GamemodeSingleton.Difficulty difficulty = GamemodeSingleton.Difficulty.Normal
		, string map = "6_Arena (Unity)"/*GamemodeSingleton.Maps map = GamemodeSingleton.Maps.DefaultMap*/
		, GamemodeSingleton.Maxplayers MaxPlayers = GamemodeSingleton.Maxplayers.four
		)
	{
		if (canJoin)
		{
			//Create Hashtable for custom lobby/room properties
			ExitGames.Client.Photon.Hashtable roomSettings = new ExitGames.Client.Photon.Hashtable()
			{
				{ "name", name }, { "mode", mode }, { "rounds", rounds }, { "difficulty", difficulty}, { "map", map }
			};

			string[] lobbyproperties = { "name", "mode", "rounds", "difficulty", "map" };

			RoomOptions options = new RoomOptions();
			options.MaxPlayers = Convert.ToByte((int)MaxPlayers);
			options.PublishUserId = true;
			options.CustomRoomPropertiesForLobby = lobbyproperties;
			options.CustomRoomProperties = roomSettings;
            PhotonNetwork.CreateRoom(null, options, TypedLobby.Default);
        }
	}

	public void JoinRandomRoom()
	{
		if (canJoin && ConnectedToLobby)
		{
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public void UpdateRoomName(string name)
	{
		if (name.Length > 0)
		{
			RoomSettings.RoomName = name;
		}
		else
		{
			RoomSettings.RoomName = "Default Server";
		}
	}

	public void UpdateRoomMaxPlayers(int count)
	{
		count++;
		if (count.ToString().Length > 0)
		{
			RoomSettings.MaxPlayers = Convert.ToByte(int.Parse(count.ToString()) == 1 ? 4 : 8);
		}
		else
		{
			RoomSettings.MaxPlayers = 4;
		}
	}

	public void UpdateUsername(string name = "")
	{
		if (name.Length > 0)
		{
			PlayerPrefs.SetString("Username", name);
			RoomSettings.Player_Name = PlayerPrefs.GetString("Username");
			//Debug.Log($"Update name: {name}");
			//RoomSettings.Player_Name = name;
		}
		else
		{
			Debug.Log("Deleting name");
			PlayerPrefs.DeleteKey("Username");
			RoomSettings.Player_Name = "Default User";
        }
		PlayerPrefs.Save();
		PhotonNetwork.NickName = RoomSettings.Player_Name;
	}

	private void Start()
	{
        if (!Application.isEditor)
        {
			AutoConnect = false;
        }
		PhotonNetwork.ConnectUsingSettings();
		PlayerPrefs.SetInt("PlayedRounds", 1);
		PlayerPrefs.Save();
	}

	private void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
		string name = PlayerPrefs.HasKey("Username") == null ? "" : PlayerPrefs.GetString("Username");
		nicknamefield.text = name;
		UpdateUsername(name);
	}
	/*
	if(index!<0&&index!>character.length){
		return character[index]
		} else if (index < 0)
	{
	return character[character.length]
	} else if (index > character.length)
	{

	return character[0];
	}
	*/

	public void SetScene(int SceneID)
    {
		SceneSelected = SceneID;
		MapSelectorBtn.image.sprite = SceneImages[SceneID];
    }
}
