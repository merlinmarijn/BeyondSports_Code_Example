using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class TestPlatformGeneration : MonoBehaviourPunCallbacks, IInRoomCallbacks
{


    [SerializeField] private GameObject poolContainer;
    [SerializeField] private Platform platformPrefab;

    public List<WeightedPlatformScriptableObject> WeightedPlatforms = new List<WeightedPlatformScriptableObject>();
    [SerializeField]
    private float[] Weights;

    //[System.Serializable]
    //public class pool {public GameObject prefab; public ObjectPool<Platform> _pool;}
    //public List<pool> pools;

    public List<Platform> platforms = new List<Platform>();

    public ObjectPool<Platform> _pool;
    public List<ObjectPool<Platform>> __pools;

    private GameObject HighestPlatform;
    private GameObject LowestPlatform;
    Vector3 spawnPosition;

    Platform PlatformToSpawn;

    bool canSpawn = false;

    private void Awake()
    {
        //Debug.Log(pools.Count);
        Weights = new float[WeightedPlatforms.Count];
        PlatformToSpawn = platforms[0].GetComponent<Platform>();
        //for (int i = 0; i < pools.Count; i++)
        //{
        //    pools[i]._pool = new ObjectPool<Platform>(() => SpawnPlatform(pools[i].prefab.name), OnTakeFromPool, OnReleaseFromPool, OnDestroyFromPool, true, 10, 10000);
        //}
        //Debug.Log(pools[0]._pool.Get());
        //Debug.Log(pools[1]._pool.Get());
        //Debug.Log(pools[2]._pool.Get());

    }

    // Start is called before the first frame update
    void Start()
    {
        __pools = new List<ObjectPool<Platform>>();

        //_pool = new ObjectPool<Platform>(()=>SpawnPlatform(PlatformToSpawn.gameObject.name), OnTakeFromPool, OnReleaseFromPool, OnDestroyFromPool, true, 10, 10000);
        for (int i = 0; i < platforms.Count; i++)
        {
            __pools.Add(new ObjectPool<Platform>(SpawnPlatform, OnTakeFromPool, OnReleaseFromPool, OnDestroyFromPool, true, 10, 10000));
            //__pools[i] = new ObjectPool<Platform>(()=>SpawnPlatform(), OnTakeFromPool, OnReleaseFromPool, OnDestroyFromPool, true, 10, 10000);
        }
        canSpawn = true;

        spawnPosition = new Vector3(0, 0, 0);
        if (PhotonNetwork.IsMasterClient)
        {
            ResetSpawnWeights();
            SpawnWeightedPlatform();
            __pools[0].Get();
            //_pool.Get();
        }
        Debug.Log("CAN SPAWN NOW");
    }

    private void Update()
    {

        if(HighestPlatform != null && HighestPlatform.transform.position.y < 40)
        {
            spawnPosition.y = (HighestPlatform == null ? 0 : HighestPlatform.transform.position.y) + Random.Range(.5f, 2f);
            spawnPosition.x = Random.Range(-7f, 7f);
            ResetSpawnWeights();
            int temp = SpawnWeightedPlatform();
            __pools[temp].Get();
        }
    }


    #region SpawnObject

    Platform SpawnPlatform()
    {
        int index = SpawnWeightedPlatform();
        Platform obj = PhotonNetwork.InstantiateSceneObject(platforms[index].gameObject.name, new Vector3(0, 10, 0), Quaternion.identity).GetComponent<Platform>();
        obj.gameObject.name = platforms[index].gameObject.name;
        obj.transform.parent = poolContainer.transform;
        return obj;
    }

    #endregion

    #region On_Take_From_Pool
    void OnTakeFromPool(Platform p)
    {
        //Debug.Log(PlatformToSpawn);
        photonView.RPC("RPC_OnTake", RpcTarget.All, p.GetComponent<PhotonView>().ViewID, spawnPosition.x, spawnPosition.y);
    }

    [PunRPC]
    void RPC_OnTake(int id, float X, float Y)
    {
        GameObject platform = PhotonView.Find(id).gameObject;
        platform.transform.position = new Vector3(X,Y,0);
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
            foreach(GameObject item in GameObject.FindGameObjectsWithTag("Platform"))
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
        for(int i = 0; i < WeightedPlatforms.Count; i++)
        {
            Weights[i] = WeightedPlatforms[i].GetWeight();
            TotalWeight += Weights[i];
        }

        for(int i = 0; i <Weights.Length; i++)
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


}
