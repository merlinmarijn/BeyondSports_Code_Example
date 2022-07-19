using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeSingleton : MonoBehaviour
{
    public static GamemodeSingleton Instance { get; set; }

    public enum Gamemodes { Team, Solo }
    public enum Rounds { One = 1, Two = 2, Three = 3, Four = 4 }
    public enum Difficulty { Easy = 0, Normal = 1, Hard = 2, Insane = 3 }
    public enum Maps { DefaultMap, SuperMap, UberMap }
    public enum Maxplayers { four = 4, eight = 8, sixteen = 16 }
    public enum Team { T1, T2, T3, T4 }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
