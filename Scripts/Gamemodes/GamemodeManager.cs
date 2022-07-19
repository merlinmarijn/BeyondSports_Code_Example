using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using System.Linq;

public class GamemodeManager : MonoBehaviourPunCallbacks
{
    public GamemodeSingleton.Gamemodes GM;
    public GamePhotonManager GPM;
    private List<Team> Teams = new List<Team>(4);

    private void Awake()
    {
        #region variable_checks
        //Debug check if all settings has been received
        //Debug.Log("Gamemode: " + (GamemodeSingleton.Gamemodes)PhotonNetwork.CurrentRoom.CustomProperties["mode"]);
        //Debug.Log("Rounds: " +(int)(GamemodeSingleton.Rounds)PhotonNetwork.CurrentRoom.CustomProperties["rounds"]);
        //Debug.Log("Difficulty: " +(GamemodeSingleton.Difficulty)PhotonNetwork.CurrentRoom.CustomProperties["difficulty"]);
        //Debug.Log("Map: " +(GamemodeSingleton.Maps)PhotonNetwork.CurrentRoom.CustomProperties["map"]);
        //Debug.Log("Max Players: " +PhotonNetwork.CurrentRoom.MaxPlayers);
        #endregion
        Set_Gamemode();
        Init_Teams();
        //if(PhotonNetwork.IsMasterClient)
        //AllocatePlayer(PhotonNetwork.MasterClient);
        //if (PhotonNetwork.IsMasterClient)
        //photonView.RPC("RPC_SyncTeam", RpcTarget.Others, Teams);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log(PhotonNetwork.LocalPlayer);
            //Debug.Log(PhotonNetwork.MasterClient.ActorNumber);
            //Debug.Log(AllocatePlayer(PhotonNetwork.MasterClient.ActorNumber).team);
            //photonView.RPC("RPC_SyncTeam", RpcTarget.MasterClient, AllocatePlayer(PhotonNetwork.MasterClient.ActorNumber));
        }
    }

    private void Update()
    {
        //EMPTY
    }

    #region GamemodeSetter

    private void Set_Gamemode()
    {
        GM = (GamemodeSingleton.Gamemodes)PhotonNetwork.CurrentRoom.CustomProperties["mode"];
    }

    #endregion


    #region TeamCode

    private void Init_Teams()
    {
        for(int i = 0; i < System.Enum.GetNames(typeof(GamemodeSingleton.Team)).Length; i++)
        {
            Teams.Add(new Team());
            Teams[i].team = (GamemodeSingleton.Team)i;
        }
    }

    //REMOVE THIS DO FROM LOCAL PLAYER
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if(PhotonNetwork.IsMasterClient)
    //    AllocatePlayer(newPlayer);
    //}


    //CHECK THIS LATER AND REWORK
    [PunRPC]
    private void RPC_AllocatePlayer(Player playerID, int viewID)
    {
        List<int> playercount = new List<int>();
        foreach(Team item in Teams)
        {
            playercount.Add(item.GetPlayerCount());
        }
        int index = playercount.IndexOf(playercount.Min());
        Teams[index].playersID.Add(playerID.ActorNumber);
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID.ActorNumber);
        for(int i = 0; i < Teams.Count; i++)
        {
            for(int o = 0; o < Teams[i].playersID.Count; o++)
            {
                photonView.RPC("RPC_SyncTeam", RpcTarget.Others, i, Teams[i].playersID[o]);
            }
        }
        photonView.RPC("RPC_SetPlayerTeam", RpcTarget.All, index, viewID);

        return;
    }


    //KEEP THIS
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("RPC_RemovePlayer", RpcTarget.All, otherPlayer.ActorNumber);
    }

    [PunRPC]
    private void RPC_RemovePlayer(int playerID)
    {
        foreach(Team item in Teams)
        {
            if (item.playersID.Contains(playerID))
            {
                int index = item.playersID.IndexOf(playerID);
                item.playersID.RemoveAt(index);
            }
        }
    }


    //CHECK IF I STILL NEED THIS
    [PunRPC]
    void RPC_SyncTeam(int index, int playerID)
    {
        Debug.Log("TEAMS SYNCED");
        Teams[index].playersID.Add(playerID);
    }


    //CHECK IF CAN BE REMOVED/REWORKED
    [PunRPC]
    void RPC_RequestLocalTeam(Player player, int id)
    {
        foreach(Team item in Teams)
        {
            if (item.playersID.Contains(player.ActorNumber))
            {
                int TeamIndex = Teams.IndexOf(item);
                photonView.RPC("RPC_SetPlayerTeam", player, TeamIndex, id);
            }
        }
    }


    //CHECK IF CAN BE REMOVED/REWORKED
    [PunRPC]
    void RPC_SetPlayerTeam(int TeamIndex, int id)
    {
        //Debug.Log(PhotonView.Find(id).gameObject.name);
        PhotonView.Find(id).GetComponent<PlayerData>().Set_Team(TeamIndex);
        //GPM.getPlayer().GetComponent<PlayerData>().Set_Team(TeamIndex);
    }

    #endregion
}


public class Team : MonoBehaviourPunCallbacks
{
    public GamemodeSingleton.Team team;
    public List<int> playersID = new List<int>(0);

    public int GetPlayerCount()
    {
        return playersID.Count;
    }
}
