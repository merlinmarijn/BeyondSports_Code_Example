using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonItemSpawner : MonoBehaviourPunCallbacks
{
    GameItemsHandle Handle;

    private void Start()
    {
        Handle = GameObject.FindGameObjectWithTag("ItemHandler").GetComponent<GameItemsHandle>();
    }

    void SpawnItem(Vector3 Pos, int itemID)
    {
        //Checks if this is master client incase so that only the owner of the room can spawn items in!
        if (PhotonNetwork.IsMasterClient)
        {
            int index = Random.Range(0, Handle.AllItems.Length - 1);
            PhotonNetwork.Instantiate(Handle.AllItems[index].name, Pos, Quaternion.identity);
        }
    }
}
