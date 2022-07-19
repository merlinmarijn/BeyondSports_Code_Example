using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    
    // public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    // private static int nextSpawnerId = 1;

    // public int spawnerId;
    // public bool hasItem = false;

    // private void Start(){
    //     hasItem = false;
    //     spawnerId = nextSpawnerId;
    //     nextSpawnerId++;
    //     spawners.Add(spawnerId, this);
    // }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player"){
            if(other.GetComponent<PlayerItem>().HeldItem == -1 && other.GetComponent<PlayerItem>().CanPickup){
                other.GetComponent<PlayerItem>().StartPickup();
                Destroy(this.gameObject);
            }
        }
    }
}
