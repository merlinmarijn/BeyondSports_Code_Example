using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FocusPlayer : MonoBehaviourPunCallbacks
{
    public float HalfXBounds = 20f;
    public float HalfYBounds = 15f;
    public float HalfZBounds = 15f;

    public Bounds FocusBounds;

    private SupCam SC;

    private void Awake()
    {
        SC = Camera.main.GetComponent<SupCam>();
        SC.AddPlayer(gameObject);
        //if (!photonView.IsMine)
        //{
        //    this.enabled = false;
        //}
        if (GetComponent<PhotonView>().Owner == PhotonNetwork.MasterClient)
        {
            SC.focusPlayer = this;
        }
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = gameObject.transform.position;
        Bounds bounds = new Bounds();
        bounds.Encapsulate(new Vector3(position.x - HalfXBounds, position.y - HalfYBounds, position.z - HalfZBounds));
        bounds.Encapsulate(new Vector3(position.x + HalfXBounds, position.y + HalfYBounds, position.z + HalfZBounds));
        FocusBounds = bounds;
    }

    private void LateUpdate()
    {
        if (!SC.focusPlayer)
        {
            foreach (GameObject item in SC.players)
            {
                if (item.GetComponent<PhotonView>().Owner == PhotonNetwork.MasterClient)
                {
                    SC.focusPlayer = item.GetComponent<FocusPlayer>();
                }
            }
        }
    }
}
