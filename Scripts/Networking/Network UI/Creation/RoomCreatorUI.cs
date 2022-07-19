using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RoomCreatorUI : MonoBehaviour
{
    GamemodeSingleton GM;
    public TMP_Dropdown Modes;
    public TMP_Dropdown Rounds;
    public TMP_Dropdown Difficulty;
    public TMP_Dropdown Maps;
    public TMP_Dropdown MaxPlayers;

    private void Start()
    {
        foreach (GamemodeSingleton.Gamemodes modes in Enum.GetValues(typeof(GamemodeSingleton.Gamemodes)))
        {
            Modes.options.Add(new TMP_Dropdown.OptionData() { text = modes.ToString() });
        }
        foreach (GamemodeSingleton.Rounds rounds in Enum.GetValues(typeof(GamemodeSingleton.Rounds)))
        {
            Rounds.options.Add(new TMP_Dropdown.OptionData() { text = rounds.ToString() });
        }
        foreach (GamemodeSingleton.Difficulty difficulty in Enum.GetValues(typeof(GamemodeSingleton.Difficulty)))
        {
            Difficulty.options.Add(new TMP_Dropdown.OptionData() { text = difficulty.ToString() });
        }
        //foreach (GamemodeSingleton.Maps map in Enum.GetValues(typeof(GamemodeSingleton.Maps)))
        //{
        //    Maps.options.Add(new TMP_Dropdown.OptionData() { text = map.ToString() });
        //}
        foreach (GamemodeSingleton.Maxplayers player in Enum.GetValues(typeof(GamemodeSingleton.Maxplayers)))
        {
            MaxPlayers.options.Add(new TMP_Dropdown.OptionData() { text = player.ToString() });
        }
    }
}
