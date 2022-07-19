using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerPlayerAttack : MonoBehaviour
{
    public float Distance = 1.5f;
    private AudioHandler audio;

    private void Start()
    {
        audio = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<AudioHandler>();
    }

    private void Update()
    {
            float HorizontalAim = Input.GetAxisRaw("HorizontalAttack");
            float VerticalAim = Input.GetAxisRaw("VerticalAttack");
            Vector3 dir = new Vector3(HorizontalAim, VerticalAim, 0).normalized;
            int layermask = ~LayerMask.GetMask("Player");
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dir, Distance, layermask);
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (raycastHit2D.collider != null)
                {
                    if (raycastHit2D.transform.tag == "Player")
                    {
                        audio.PlaySound(0, gameObject, 0.05f);
                    //if ((GamemodeSingleton.Gamemodes)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == GamemodeSingleton.Gamemodes.Solo ||
                    //    raycastHit2D.transform.GetComponent<PlayerData>().team != transform.GetComponent<PlayerData>().team &&
                    //    (GamemodeSingleton.Gamemodes)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == GamemodeSingleton.Gamemodes.Team
                    //    )
                    //{
                    //    photonView.RPC("RPC_CallKnockback", raycastHit2D.transform.GetComponent<PhotonView>().Controller, raycastHit2D.transform.GetComponent<PhotonView>().ViewID, 1f);
                    //}
                    //ADD HERE KNOCKBACK FOR SINGLEPLAYER
                    raycastHit2D.transform.GetComponent<SingleplayerPlayerMovement>().Knockback(transform.position, 1f);
                    }
                }
            }
            Debug.DrawRay(transform.position, dir, Color.red);
    }
}
