using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemAlskar : MonoBehaviourPun
{
    [Tooltip("Is it time to throw some hail down on people?")]
    [SerializeField] public bool hailTime;

    [Tooltip("Is hail coming down?")]
    [SerializeField] private bool hailing;

    [Tooltip("The speed at which the hail goes")]
    [SerializeField] private float hailSpeed = 0.05f;

    [Tooltip("Spawn radius of the hailstorm on X level")]
    [SerializeField] private float spawnRadius = 5f;

    private float minSpawnX;
    private float maxSpawnX;

    private GameObject[] hails;
    private ParticleSystem snowEffect;

    private float[] spawnY;

    void Start()
    {
        spawnY = new float[transform.childCount-1];
        hails = new GameObject[transform.childCount-1];
        snowEffect = transform.GetChild(3).GetComponent<ParticleSystem>();;
        GetChildren();
        minSpawnX = transform.parent.position.x - spawnRadius;
        maxSpawnX = transform.parent.position.x + spawnRadius;

        for(int i = 0; i < transform.childCount-1; i++)
            {
                spawnY[i] = transform.GetChild(i).transform.position.y;
            }

        foreach(GameObject child in hails)
            {
                child.SetActive(false);
            }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.J))
        StartHailing();
    }

    void FixedUpdate() 
    {
       if (hails.Length > 1 && hailing)
        {
            foreach(GameObject child in hails)
            {
                if (child.transform.localPosition.y >= transform.parent.position.y - spawnY[0] && hailing)
                    child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y - hailSpeed, child.transform.localPosition.z);
                else
                    child.SetActive(false);
            }
        }

        if (allInActive() && hailTime)
        {
            photonView.RPC("StartHailing", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartHailing() 
    {
        hailing = false;
        if (!hailTime)
            {
            hailTime = !hailTime;
            SnowEffect(hailTime);
                if (hails.Length > 1)
                {
                    for (int i = 0; i < hails.Length; i++)
                    {
                        hails[i].transform.localPosition = new Vector3(Random.Range(minSpawnX, maxSpawnX), spawnY[i], 0);
                        hails[i].gameObject.SetActive(hailTime);
                    }
                    StartCoroutine("HailDown");
                }
            }
            else
                hailTime = !hailTime;
    }
    IEnumerator HailDown()
    {
        int time = 3;
        yield return new WaitForSeconds(time);
        hailing = hailTime;
    }

        void SnowEffect(bool emit)
    {
        if (emit)
        {
            snowEffect.enableEmission = true;
            snowEffect.Play();
        }
        if (!emit && snowEffect.enableEmission)
        {
            snowEffect.enableEmission = false;
            snowEffect.Stop();
        }
    }

      public bool allInActive()
    {
        bool response = true;
        for (int i = 0; i < hails.Length; i++)
        {
            if (hails[i].activeInHierarchy)
            {
                response = false;
                break;
            }
        }
        return response;
    }

    private void GetChildren()
    {
        for (int i = 0; i <= transform.childCount-2; i++){
            hails[i] = transform.GetChild(i).gameObject;
        }
    }
}
