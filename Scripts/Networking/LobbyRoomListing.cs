using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LobbyRoomListing : MonoBehaviourPunCallbacks
{
	[Header("Button Prefab")]
	public GameObject Button;

	[Header("Container for server listings")]
	public GameObject ListingContainer;

	private List<ListingServerButton> Listings = new List<ListingServerButton>();

	[Header("Containers for Filter buttons")]
	[SerializeField] public TMP_Dropdown modeDropdown;
	[SerializeField] public TMP_Dropdown roundsDropdown;
	[SerializeField] public TMP_Dropdown difficultyDropdown;
	[SerializeField] public TMP_Dropdown mapDropdown;
	[SerializeField] public TMP_Dropdown maxplayersDropdown;


    private void Start()
    {
		//Populate Filter buttons with ENUMS as options
		foreach (GamemodeSingleton.Gamemodes modes in Enum.GetValues(typeof(GamemodeSingleton.Gamemodes)))
		{
			modeDropdown.options.Add(new TMP_Dropdown.OptionData() { text = modes.ToString() });
		}
		foreach (GamemodeSingleton.Rounds rounds in Enum.GetValues(typeof(GamemodeSingleton.Rounds)))
		{
			roundsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = ((int)rounds).ToString() });
		}
		foreach (GamemodeSingleton.Difficulty difficulty in Enum.GetValues(typeof(GamemodeSingleton.Difficulty)))
		{
			difficultyDropdown.options.Add(new TMP_Dropdown.OptionData() { text = difficulty.ToString() });
		}
		foreach (GamemodeSingleton.Maps map in Enum.GetValues(typeof(GamemodeSingleton.Maps)))
		{
			mapDropdown.options.Add(new TMP_Dropdown.OptionData() { text = map.ToString() });
		}
		foreach (GamemodeSingleton.Maxplayers player in Enum.GetValues(typeof(GamemodeSingleton.Maxplayers)))
		{
			maxplayersDropdown.options.Add(new TMP_Dropdown.OptionData() { text = ((int)player).ToString() });
		}
	}


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (RoomInfo info in roomList)
		{
			if (info.RemovedFromList)
			{
				int num = Listings.FindIndex((ListingServerButton x) => x.RoomInfo.Name == info.Name);
				if (num != -1)
				{
					UnityEngine.Object.Destroy(Listings[num].gameObject);
					Listings.RemoveAt(num);
				}
			}
			else
			{
				int num2 = Listings.FindIndex((ListingServerButton x) => x.RoomInfo.Name == info.Name);
				if (num2 != -1)
				{
					Listings[num2].SetRoomInfo(info);
				}
				else
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Button, ListingContainer.transform);
					ListingServerButton component = gameObject.GetComponent<ListingServerButton>();
					if (component != null)
					{
						component.SetRoomInfo(info);
						Listings.Add(component);
					}
				}
			}
		}
	}


    #region filtering_servers
    //Filter based on {Gamemode, rounds, difficulty, map, max players}
    public void FilterServers()
    {
		foreach (ListingServerButton button in Listings) 
		{
			RoomInfo info = button.RoomInfo;
            if (CheckGamemode(info) && CheckRounds(info) && CheckDifficulty(info) && CheckMap(info) && CheckMaxPlayers(info))
            {
				button.gameObject.SetActive(true);
            } else
            {
				button.gameObject.SetActive(false);
            }
		}
    }

    #region FilterCheck
	private bool CheckGamemode(RoomInfo info)
    {
		if (modeDropdown.value == 0) { return true; }
		if (((GamemodeSingleton.Gamemodes)info.CustomProperties["mode"]).ToString() != modeDropdown.options[modeDropdown.value].text) { return false; }
		return true;
	}
	private bool CheckRounds(RoomInfo info)
    {
		if (roundsDropdown.value == 0) { return true; }
		if ((int)info.CustomProperties["rounds"] != int.Parse(roundsDropdown.options[roundsDropdown.value].text)) { return false; }
		return true;
	}
	private bool CheckDifficulty(RoomInfo info)
    {
		if (difficultyDropdown.value == 0) { return true; }
		if (((GamemodeSingleton.Difficulty)info.CustomProperties["difficulty"]).ToString() != difficultyDropdown.options[difficultyDropdown.value].text) { return false; }
		return true;
	}
	private bool CheckMap(RoomInfo info)
    {
		if (mapDropdown.value == 0) { return true; }
		if (((GamemodeSingleton.Maps)info.CustomProperties["map"]).ToString() != mapDropdown.options[mapDropdown.value].text) { return false; }
		return true;
	}
	private bool CheckMaxPlayers(RoomInfo info)
    {
        if (maxplayersDropdown.value == 0) { return true; }
		if (info.MaxPlayers != int.Parse(maxplayersDropdown.options[maxplayersDropdown.value].text)) { return false; }
		return true;
    }

	#endregion

	public void ResetFilter()
	{
		modeDropdown.value = 0;
		roundsDropdown.value = 0;
		difficultyDropdown.value = 0;
		mapDropdown.value = 0;
		maxplayersDropdown.value = 0;
		FilterServers();
	}

	#endregion

}
