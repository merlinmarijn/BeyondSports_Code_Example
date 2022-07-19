using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemHalcyon : MonoBehaviourPun
{
    private Vector3 leftPos;
    private Vector3 rightPos;
    private bool goRight;
    private bool goLeft;
    private SpriteRenderer tsuSprite;

    [Tooltip("The speed of the tsunami")]
    [SerializeField] private float moveSpeed = 0.1f;
    [Tooltip("Start the tsunami wave")]
    [SerializeField] public bool startWave;

    private BoxCollider2D halBox;

    void Start()
    {
        leftPos = this.transform.localPosition;
        rightPos = new Vector3(Mathf.Abs(leftPos.x), leftPos.y, leftPos.z);
        tsuSprite = GetComponent<SpriteRenderer>();
        halBox = GetComponent<BoxCollider2D>();
        halBox.enabled = false;
        tsuSprite.enabled = false;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.H))
        DecideSide();

    if (Input.GetKeyDown(KeyCode.P)) 
    Debug.Log("DecideSide: " + startWave + " halBox: " + halBox.enabled + " tsusprite: " + tsuSprite.enabled + " goRight: " + goRight + " goLeft: " + goLeft);

    }

    void FixedUpdate()
    {
        if (transform.localPosition.x <= rightPos.x && goRight && startWave)
            transform.localPosition = new Vector3(transform.localPosition.x + moveSpeed, transform.localPosition.y, transform.localPosition.z);
        else
            goRight = false;

        if (transform.position.x >= leftPos.x && goLeft && startWave)
            transform.localPosition = new Vector3(transform.localPosition.x - moveSpeed, transform.localPosition.y, transform.localPosition.z);
        else
            goLeft = false;

        if (!goLeft && !goRight && startWave)
            photonView.RPC("DecideSide", RpcTarget.All);
    }

    [PunRPC]
    public void HalcyonVar(bool startBool) 
    {
        startWave = startBool;
        int rndSide = Random.Range(0, 2);

        tsuSprite.enabled = startBool;
        halBox.enabled = startBool;

        if (rndSide == 0 && startWave)
        {
            this.transform.localPosition = leftPos;
            this.transform.localEulerAngles = new Vector3(0, 0, 0);
            goRight = true;
        }

        if (rndSide == 1 && startWave) 
        {
            this.transform.localPosition = rightPos;
            this.transform.localEulerAngles = new Vector3(0, 180, 0);
            goLeft = true;
        }
    }

    [PunRPC]
    public void DecideSide()
    {
        if (!startWave)
            {
                startWave = !startWave;
                photonView.RPC("HalcyonVar", RpcTarget.All, startWave);
            }
        else
        startWave = !startWave;    
    }
}
