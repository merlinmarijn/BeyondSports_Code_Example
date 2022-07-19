using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Platform : MonoBehaviourPunCallbacks
{
    public float MovementSpeed = 10f;

    private PlatformGeneration TPG;
    public bool DestroyThis = false;

[Tooltip("Is this platform a Kafara platform?")]
    [SerializeField] private bool disKafara = false;
    
    [Tooltip("Chances of becoming a Kafara platform, 1 to --")]
    [SerializeField] private int disChance = 5;
    public string tag;

    Rigidbody2D RB2D;

    private void Awake()
    {
        TPG = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<PlatformGeneration>();
        RB2D = GetComponent<Rigidbody2D>();
        MovementSpeed += ((MovementSpeed/2) * (int)PhotonNetwork.CurrentRoom.CustomProperties["difficulty"]);
        //InvokeRepeating("OutOfPlayerRange", 0, 10);
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.Translate((Vector3.down * Time.deltaTime)*MovementSpeed);
        }
        if (transform.position.y <= Camera.main.GetComponent<SupCam>().transform.position.y - 15)
        {
           ReleaseToPool();
        }

        if (disKafara)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player"))
                ReleaseToPool();
            }
        }
    }

    [PunRPC]
    public void KafaraPlatform()
    {
        int rnd = Random.Range(1,disChance);
        if (rnd == 2)
        disKafara = !disKafara;
    }

    void ReleaseToPool()
    {
            if (DestroyThis)
            {
                Destroy(gameObject);
                disKafara = false;
            }
            else
            {
                int index = 0;
                for (int i = 0; i < TPG.WeightedPlatforms.Count; i++)
                {
                    if (this.tag == TPG.WeightedPlatforms[i].platform.tag)
                    {
                        index = i;
                    }
                }
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("Player"))
                    child.parent = null;
                }
                disKafara = false;
                TPG.__pools[index].Release(this);
            }
    }

    void OutOfPlayerRange()
    {
        if(transform.position.y <= Camera.main.GetComponent<SupCam>().transform.position.y - 25)
        {
            ReleaseToPool();
        }
    }
}