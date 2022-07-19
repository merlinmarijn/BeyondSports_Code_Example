using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerData : MonoBehaviourPunCallbacks
{

    public GamemodeSingleton.Team team;
    private GamePhotonManager GPM;
    private GamemodeManager GM;


    private void Start()
    {
        GM = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<GamemodeManager>();
        //GetComponent<PlayerMovement2D>().GPM.GetComponent<GamemodeManager>()
        if (photonView.IsMine)
        {
            RequestTeam();
        } else
        {
            photonView.RPC("RPC_RequestSyncTeam", photonView.Owner, PhotonNetwork.LocalPlayer);
        }
    }


    //CHECK IF CAN BE REMOVED/REWORKED
    public void Set_Team(int index)
    {
        team = (GamemodeSingleton.Team)index;
    }

    [PunRPC]
    public void RPC_Set_Team(int index)
    {
        team = (GamemodeSingleton.Team)index;
    }


    public void RequestTeam()
    {
        if (photonView.IsMine)
        {
            GM.photonView.RPC("RPC_AllocatePlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, photonView.ViewID);
        }
    }

    [PunRPC]
    public void RPC_RequestSyncTeam(Player player)
    {
        //THIS IS MASTER CLIENT SIDE >>> REQUESTING CLIENT
        int index = (int)team;
        photonView.RPC("RPC_Set_Team", player, index);
    }

    public void setGameManager(GamePhotonManager gameManager)
    {
        GPM = gameManager;
    }
}
