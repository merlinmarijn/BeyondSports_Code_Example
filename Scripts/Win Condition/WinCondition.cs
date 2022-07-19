using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviourPunCallbacks
{
    GamePhotonManager GPM;
    JailSystem JS;
    List<int>TeamPlayerCount;
    private int thisPlayerTeam;

    private void Start()
    {
        JS = GameObject.FindGameObjectWithTag("Jail").GetComponent<JailSystem>();
        GPM = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<GamePhotonManager>();
        TeamPlayerCount = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            TeamPlayerCount.Add(0);
        }
        Invoke(nameof(UpdatePlayerCount), 1f);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //Check if a team has failed
    public void CheckJail()
    {
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == (int)GamemodeSingleton.Gamemodes.Team)
        {
            int TeamsLost = 0;
            for (int i = 0; i < TeamPlayerCount.Count; i++)
            {
                if (TeamPlayerCount[i] == JS.JailQueue[i].Count && TeamPlayerCount[i] != 0)
                {
                    TeamsLost++;
                    if (i == thisPlayerTeam)
                        Debug.Log("LOST");

                    if (TeamsLost == TeamPlayerCount.Count-1)
                    {
                        Debug.Log("Round ended");
                        if (PhotonNetwork.IsMasterClient)
                        {
                            //Check rounds and then do when rounds are played
                            CheckRounds();
                        }
                    }
                }
            }
        } else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == (int)GamemodeSingleton.Gamemodes.Solo)
        {
            if(JS.JailQueue[0].Count == GameObject.FindGameObjectsWithTag("Player").Length - 1)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //Check rounds and then do when rounds are played
                    CheckRounds();
                }
            }
        }
    }

    void CheckRounds()
    {
        Debug.Log($"Rounds to play: {(int)PhotonNetwork.CurrentRoom.CustomProperties["rounds"]}, Rounds Played: {PlayerPrefs.GetInt("PlayedRounds")}");
        if (PlayerPrefs.GetInt("PlayedRounds") >= (int)PhotonNetwork.CurrentRoom.CustomProperties["rounds"])
        {
            //if played all rounds do something
            StartCoroutine(GPM.CloseRoom());
            Debug.Log("aaaaaaaaaaaaaaaaaaaaaaa");
        }
        else
        {
            Debug.Log("BBBBBBBBBBBBBBBBBBBBBB");
            //if not played all rounds do something
            PlayerPrefs.SetInt("PlayedRounds", PlayerPrefs.GetInt("PlayedRounds")+1);
            PlayerPrefs.Save();
            StartCoroutine(ReloadScene());
        }
    }

    IEnumerator ReloadScene()
    {
        photonView.RPC("LoadScene", RpcTarget.Others, SceneManager.GetActiveScene().name);
        yield return null;
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }

    [PunRPC]
    public void LoadScene(string SceneName)
    {
        PhotonNetwork.LoadLevel(SceneName);
    }

    private void Update()
    {
    }

    #region OnPlayerEntered/Left

    //This is keeping track of how many people are in each team when someone connects
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Invoke(nameof(UpdatePlayerCount), 1f);
    }

    //This is keeping track of how many people are in each team when someone disconnects
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Invoke(nameof(UpdatePlayerCount), 1f);
    }


    public void UpdatePlayerCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<int> tempcount = new List<int>(players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            tempcount.Add(0);
        }
        foreach (GameObject item in players)
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == (int)GamemodeSingleton.Gamemodes.Team)
            {
                tempcount[(int)item.GetComponent<PlayerData>().team]++;
            } else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == (int)GamemodeSingleton.Gamemodes.Solo)
            {
                tempcount[0]++;
            }
            if (item.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                thisPlayerTeam = (int)item.GetComponent<PlayerData>().team;
        }
        TeamPlayerCount = tempcount;
    }

    #endregion

}
