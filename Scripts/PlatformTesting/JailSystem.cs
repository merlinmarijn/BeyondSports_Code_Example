using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class JailSystem : MonoBehaviourPunCallbacks
{
    //Out of bounds jail
    public Transform JailObj;
    //Queue of players in jail
    public Queue[] JailQueue = new Queue[4];
    public GameObject Unjailplatform;
    public Transform UnjailTransform;
    WinCondition wc;

    //THIS IS MASTERCLIENT PERSPECTIVE
    //SYNC QUEUE WITH EVERYONE BUT ONLY LET MASTERCLIENT DO SOMETHING WITH IT
    //THIS WAY EVERYONE HAS A COPY OF THE QUEUE BUT MASTERCLIENT CAN ONLY JAIL/UNJAIL PLAYERS AND THEN RESYNCS QUEUE


    void Update()
    {
        //new Vector3 Pos = 
        //UnjailTransform.position =
    }

    private void Awake()
    {
        for(int i = 0; i < JailQueue.Length; i++)
        {
            JailQueue[i] = new Queue();
        }
        wc = GetComponent<WinCondition>();
        InvokeRepeating(nameof(RepositionUnjailPlatform), 1, 1);
        //Debug.Log(JailQueue[0]);
    }


    void RepositionUnjailPlatform()
    {
        if (!Unjailplatform.active)
        {
            float PlatformHeight = new float();
            for (int i = 0; i < Camera.main.GetComponent<SupCam>().players.Count; i++)
            {
                PlatformHeight += Camera.main.GetComponent<SupCam>().players[i].transform.position.y;
            }
            float temp = PlatformHeight / Camera.main.GetComponent<SupCam>().players.Count;
            Unjailplatform.transform.position = new Vector3(-5, temp, 0);
        }
    }

    public void GetJailed(int id, int team)
    {
        //Debug.Log("GET JAILED");
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == (int)GamemodeSingleton.Gamemodes.Team) 
        {
            photonView.RPC("RPC_JailQueue", RpcTarget.MasterClient, id, team);
        } else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == (int)GamemodeSingleton.Gamemodes.Solo)
        {
            photonView.RPC("RPC_JailQueue", RpcTarget.MasterClient, id, 0);
        }
    }

    public void GetUnjailed(int team)
    {
        photonView.RPC("RPC_UnQueue", RpcTarget.MasterClient, team);
    }

    //Set Player in jail and queue them up
    //EVERYONE save queue
    [PunRPC]
    void RPC_JailQueue(int id, int team)
    {
        //Debug.Log("JAIL QUEUE");
        JailQueue[team].Enqueue(id);
        Player player = PhotonView.Find(id).gameObject.GetComponent<PhotonView>().Owner;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_JailPlayer", player, id);
            photonView.RPC("RPC_SyncQueue", RpcTarget.Others, team, id);

        }
        wc.CheckJail();
    }

    //Player go to jail
    [PunRPC]
    void RPC_JailPlayer(int id)
    {
        //Debug.Log("Jail Player");
        GameObject player = PhotonView.Find(id).gameObject;
        player.transform.position = JailObj.transform.position;
        //Disable player movement!
        player.GetComponent<PlayerMovement2D>().enabled = false;
        //player.GetComponent<PlayerAttack>().enabled = false;
    }


    //Master client unqueue
    [PunRPC]
    void RPC_UnQueue(int team)
    {
        //Debug.Log(JailQueue[team].Count);
        //Debug.Log((int)JailQueue[team].Peek());
        //Debug.Log(PhotonView.Find((int)JailQueue[team].Peek()));
        while(PhotonView.Find((int)JailQueue[team].Peek()) == null && JailQueue[team].Count>0)
        {
            //Debug.Log("1");
                JailQueue[team].Dequeue();
        }
        if  (JailQueue[team].Count > 0)
        {
            //Debug.Log("2");
            int PlayerID = (int)JailQueue[team].Peek();
            photonView.RPC("RPC_UnjailPlayer", PhotonView.Find(PlayerID).gameObject.GetComponent<PhotonView>().Owner, PlayerID);
            JailQueue[team].Dequeue();
            //Sync Deueue'ing
            photonView.RPC("RPC_UnQueueSync", RpcTarget.Others, team);
            //Debug.Log("3");
        }
    }

    //EVERYONE SYNC QUEUE (DEQUEUE FIRST PERSON)
    [PunRPC]
    void RPC_UnQueueSync(int team)
    {
        JailQueue[team].Dequeue();
    }

    //Player Go out of jail
    [PunRPC]
    void RPC_UnjailPlayer(int id)
    {
        //Debug.Log("4");
        GameObject player = PhotonView.Find(id).gameObject;
        //PUT PLAYER ON REVIVE PLATFORM SO HE CAN PLAY AGAIN!
        //player.transform.position KOPPEL PLATFORM AAN DIT
        //Enable Playermovement!
        photonView.RPC("RPC_EnableUnjailPlatform", RpcTarget.All,true);
        player.GetComponent<PlayerMovement2D>().enabled = true;
        player.transform.position = UnjailTransform.position;
        StartCoroutine(ReassignCam(id, 1f));
        Invoke(nameof(disableUnjailPlatform), 2f);
    }

    void disableUnjailPlatform()
    {
        photonView.RPC("RPC_EnableUnjailPlatform", RpcTarget.All, false);
    }

    [PunRPC]
    void RPC_EnableUnjailPlatform(bool Active)
    {
        Unjailplatform.SetActive(Active);
    }

    IEnumerator ReassignCam(int id, float delay)
    {
        yield return new WaitForSeconds(delay);
        Camera.main.GetComponent<SupCam>().photonView.RPC("RPC_GetUnjailed", RpcTarget.All, id);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Queue item in JailQueue)
            {
                if (item.Count > 0)
                {
                    SyncQueue(newPlayer, newPlayer.ActorNumber);
                    //There is a player in queue, Sync with new player so that he has a copy
                    break;
                }
            }
        }
    }

    void SyncQueue(Player newPlayer, int id)
    {
        Debug.Log("syncqueue");
        //Create Temp queue to store data
        Queue[] tempqueue = new Queue[4];
        for (int i = 0; i < JailQueue.Length; i++)
        {
            tempqueue[i] = new Queue();
        }


        //Fill temp queue with data
        for (int i = 0; i < JailQueue.Length; i++)
        {
            while (JailQueue[i].Count > 0)
            {
                tempqueue[i].Enqueue((int)JailQueue[i].Dequeue());
            }
        }

        
        //Distribute temp queue to new player, and reassign to master client

        for(int i = 0; i < tempqueue.Length; i++)
        {
            while (tempqueue[i].Count > 0)
            {
                int PlayerID = (int)tempqueue[i].Dequeue();
                JailQueue[i].Enqueue(PlayerID);
                photonView.RPC("RPC_SyncQueue", newPlayer, i , PlayerID);
            }
        }
    }

    [PunRPC]
    void RPC_SyncQueue(int index, int id)
    {
        Debug.LogError("testadsasdasd");
        JailQueue[index].Enqueue(id);
    }

    //Sync to players who join later.
}
