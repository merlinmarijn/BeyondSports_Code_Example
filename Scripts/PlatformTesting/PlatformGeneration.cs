using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Pool;

public class PlatformGeneration : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    #region NEW_CODE

    [SerializeField] private GameObject poolContainer;
    [SerializeField] private Platform platformPrefab;

    public List<WeightedPlatformScriptableObject> WeightedPlatforms = new List<WeightedPlatformScriptableObject>();
    [SerializeField]
    private float[] Weights;


    public ObjectPool<Platform> _pool;
    public List<ObjectPool<Platform>> __pools;

    private GameObject HighestPlatform;
    private GameObject LowestPlatform;
    Vector3 spawnPosition;


    bool canSpawn = false;

    SupCam SC;

    private void Awake()
    {
        Weights = new float[WeightedPlatforms.Count];
        SC = Camera.main.GetComponent<SupCam>();
    }

    // Start is called before the first frame update
    void Start()
    {
        __pools = new List<ObjectPool<Platform>>();

        for (int i = 0; i < WeightedPlatforms.Count; i++)
        {
            int index = i;
            __pools.Add(new ObjectPool<Platform>(()=> SpawnPlatform(index), OnTakeFromPool, OnReleaseFromPool, OnDestroyFromPool, true, 10, 10)); //10000
        }
        canSpawn = true;

        spawnPosition = new Vector3(0, 0, 0);
        if (PhotonNetwork.IsMasterClient)
        {
            ResetSpawnWeights();
            SpawnWeightedPlatform();
            __pools[0].Get();
        }
    }

    private void Update()
    {        
        //Add here to the 40 the highestplayer Y cordinates
        if (SC.focusPlayer != null && HighestPlatform != null && HighestPlatform.transform.position.y < (SC.focusPlayer.gameObject.transform.position.y + 20) && PhotonNetwork.IsMasterClient)
        {
            spawnPosition.y = (HighestPlatform == null ? 0 : HighestPlatform.transform.position.y) + Random.Range(.5f, 2f);
            //Maak hier breeder van
            //Gooi check in dat die niet te dichtbij is
            //Gooi check in dat die niet te ver is
            spawnPosition.x = Random.Range(-14f, 14f);
            float distance = Vector3.Distance(HighestPlatform.transform.position, spawnPosition);
            if (distance < 4 || distance > 8)
            {
                return;
            }
            ResetSpawnWeights();
            int temp = SpawnWeightedPlatform();
            __pools[temp].Get();
        }
    }

    #region SpawnObject

    Platform SpawnPlatform(int index)
    {
        Platform obj = PhotonNetwork.InstantiateSceneObject(WeightedPlatforms[index].platform.gameObject.name, new Vector3(0, 10, 0), Quaternion.identity).GetComponent<Platform>();
        obj.gameObject.name = WeightedPlatforms[index].platform.gameObject.name;
        obj.transform.parent = poolContainer.transform;
        return obj;
    }

    #endregion

    #region On_Take_From_Pool
    void OnTakeFromPool(Platform p)
    {
        photonView.RPC("RPC_OnTake", RpcTarget.All, p.GetComponent<PhotonView>().ViewID, spawnPosition.x, spawnPosition.y);
    }

    [PunRPC]
    void RPC_OnTake(int id, float X, float Y)
    {
        GameObject platform = PhotonView.Find(id).gameObject;
        platform.transform.position = new Vector3(X, Y, 0);
        platform.SetActive(true);
        HighestPlatform = platform;
    }

    #endregion

    #region On_Release_From_Pool

    void OnReleaseFromPool(Platform p)
    {
        photonView.RPC("RPC_OnRelease", RpcTarget.All, p.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void RPC_OnRelease(int id)
    {
        GameObject platform = PhotonView.Find(id).gameObject;
        platform.SetActive(false);
    }

    #endregion

    #region On_Destroy_From_Pool

    void OnDestroyFromPool(Platform p)
    {

    }

    #endregion

    #region On_MasterClient_Switch

    void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            HighestPlatform = GameObject.FindGameObjectsWithTag("Platform").OrderByDescending(_HighestPlatform => _HighestPlatform.transform.position.y).First();
            LowestPlatform = GameObject.FindGameObjectsWithTag("Platform").OrderByDescending(_LowestPlatform => _LowestPlatform.transform.position.y).Last();
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Platform"))
            {
                item.GetComponent<Platform>().DestroyThis = true;
            }
        }
    }

    #endregion


    #region Weighted_Spawn_Functions

    private void ResetSpawnWeights()
    {
        float TotalWeight = 0;
        for (int i = 0; i < WeightedPlatforms.Count; i++)
        {
            Weights[i] = WeightedPlatforms[i].GetWeight();
            TotalWeight += Weights[i];
        }

        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Weights[i] / TotalWeight;
        }
    }

    private int SpawnWeightedPlatform()
    {
        int temp = 0;
        if (canSpawn)
        {
            float Value = Random.value;

            for (int i = 0; i < Weights.Length; i++)
            {
                if (Value < Weights[i])
                {
                    // SPAWN PLATFORM HERE
                    temp = i;
                    return i;
                }

                Value -= Weights[i];
            }
        }
        return temp;
    }

    #endregion


    #endregion

    #region OLD_CODE
    ////public GameObject platformPrefab;
    //public List<GameObject> platformPrefabs;

    //[System.Serializable]
    //public class pool { public string tag; public GameObject prefab; public int size; }
    //public List<pool> pools;
    //public Dictionary<string, Queue<GameObject> > poolDictionary;
    //public GameObject PoolContainer;

    //public int platformCount = 10;

    //[SerializeField] private GameObject objPosition;

    ////Used to empty out pool dictionary to sync with other players
    //public Dictionary<string, Queue<GameObject> > poolDictionaryTemp;


    //// Start is called before the first frame update

    //private void Awake()
    //{
    //    poolDictionary = new Dictionary<string, Queue<GameObject>>();

    //    foreach (pool items in pools)
    //    {
    //        Queue<GameObject> objectPool = new Queue<GameObject>();
    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            for (int i = 0; i < items.size; i++)
    //            {
    //                GameObject obj = PhotonNetwork.Instantiate(items.prefab.name, new Vector3(1000,1000,1000), Quaternion.identity);
    //                obj.transform.parent = PoolContainer.transform;
    //                //PhotonView.Find(obj.GetComponent<PhotonView>().ViewID).gameObject.SetActive(false);
    //                photonView.RPC("RPC_SetActive", RpcTarget.All, obj.GetComponent<PhotonView>().ViewID, false);

    //                //obj.SetActive(false);
    //                objectPool.Enqueue(obj);
    //            }
    //        }
    //        poolDictionary.Add(items.tag, objectPool);
    //    }
    //}

    //void Start()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        Vector3 spawnPosition = new Vector3(0,0,0);

    //        // If there are less then 250 Platforms, spawn platforms according to these co�rdinates in relation to the player
    //        for (int i = 0; i < platformCount; i++)
    //        {
    //            int randomPlatform = Random.Range(0, platformPrefabs.Count);

    //            spawnPosition.y = (objPosition == null ? 0 : objPosition.transform.position.y) + Random.Range(.5f, 2f);
    //            spawnPosition.x = Random.Range(-7f, 7f);
    //            //objPosition = SpawnPlatformFromPool("Platforms", spawnPosition, Quaternion.identity);
    //            //Instantiate(platformPrefabs[randomPlatform], spawnPosition, Quaternion.identity);

    //            objPosition = SpawnPlatformFromPool(pools[Random.Range(0, 3)].tag, spawnPosition, Quaternion.identity);
    //        }
    //    }
    //}

    //private void Update()
    //{
    //}

    //private void FixedUpdate()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        foreach (pool item in pools)
    //        {
    //            foreach (GameObject obj in poolDictionary[item.tag])
    //            {
    //                if (obj.transform.position.y < -20)
    //                {
    //                    photonView.RPC("RPC_SetActive", RpcTarget.All, obj.GetComponent<PhotonView>().ViewID, false);
    //                    //PhotonView.Find(obj.GetComponent<PhotonView>().ViewID).gameObject.SetActive(false);
    //                    //obj.SetActive(false);
    //                }
    //                //obj.transform.position.y -= Time.deltaTime * 5;
    //            }
    //        }

    //        if (objPosition != null && objPosition.transform.position.y < 40)
    //        {
    //            Vector3 spawnPosition = new Vector3();

    //            spawnPosition.y = objPosition.transform.position.y + Random.Range(.5f, 2f);
    //            spawnPosition.x = Random.Range(-7f, 7f);
    //            objPosition = SpawnPlatformFromPool("Platforms", spawnPosition, Quaternion.identity);
    //        }
    //    }
    //}

    //private GameObject SpawnPlatformFromPool(string tag, Vector3 position, Quaternion rotation)
    //{
    //    //Debug.Log("test");
    //    if (!poolDictionary.ContainsKey(tag))
    //    {
    //        Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
    //        return null;
    //    }

    //    GameObject objectToSpawn = poolDictionary[tag].Dequeue();
    //    //PhotonView.Find(objectToSpawn.GetComponent<PhotonView>().ViewID).gameObject.SetActive(true);
    //    photonView.RPC("RPC_SetActive", RpcTarget.All, objectToSpawn.GetComponent<PhotonView>().ViewID, true);
    //    //objectToSpawn.SetActive(true);
    //    objectToSpawn.transform.position = position;
    //    objectToSpawn.transform.rotation = rotation;
    //    poolDictionary[tag].Enqueue(objectToSpawn);
    //    photonView.RPC("RPC_UpdatePools", RpcTarget.All, tag);
    //    return objectToSpawn;
    //}

    //// wanneer je een bepaalde positie behaald, spawn je platforms uit de pool.

    //[PunRPC]
    //void RPC_SetActive(int id, bool state)
    //{
    //    PhotonView.Find(id).gameObject.SetActive(state);
    //}

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if(PhotonNetwork.IsMasterClient)
    //    CopyPools(newPlayer);
    //}

    //public void CopyPools(Player P)
    //{
    //    poolDictionaryTemp = new Dictionary<string, Queue<GameObject>>();
    //    Debug.Log("CLICKED");

    //    foreach(pool item in pools)
    //    {
    //        //Debug.Log($"Dictionary: Key: {item.tag}, Count: {poolDictionary[item.tag].Count}");
    //        Queue<GameObject> objectPool = new Queue<GameObject>();
    //        while (poolDictionary[item.tag].Count >0)
    //        {
    //            objectPool.Enqueue(poolDictionary[item.tag].Dequeue());
    //        }
    //        //Debug.Log($"Dictionary: Key: {item.tag}, Count: {poolDictionary[item.tag].Count}");
    //        //Debug.Log(item.tag);
    //        //Debug.Log($"pool: {objectPool.Count}");
    //        poolDictionaryTemp.Add(item.tag, objectPool);
    //        //Debug.Log($"Dictionary temp: Key: {item.tag}, Count: {poolDictionaryTemp[item.tag].Count}");
    //    }
    //    foreach(pool item in pools)
    //    {
    //        Queue<GameObject> objectPool = new Queue<GameObject>();
    //        while (poolDictionaryTemp[item.tag].Count > 0)
    //        {
    //            GameObject platform = poolDictionaryTemp[item.tag].Dequeue();
    //            poolDictionary[item.tag].Enqueue(platform);
    //            photonView.RPC("RPC_SyncPools", P, item.tag, platform.GetComponent<PhotonView>().ViewID);
    //        }
    //    }

    //}

    //[PunRPC]
    //public void RPC_SyncPools(string tag, int ID)
    //{
    //    poolDictionary[tag].Enqueue(PhotonView.Find(ID).gameObject);
    //}

    //[PunRPC]
    //public void RPC_UpdatePools(string tag)
    //{
    //    GameObject temp = poolDictionary[tag].Dequeue();
    //    poolDictionary[tag].Enqueue(temp);
    //}

    #endregion
}
