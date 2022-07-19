using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    public float Distance = 1.5f;
    private AudioHandler audio;
    GamePhotonManager GPM;
    
    InputManger inputManager;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManger>();
        GPM = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<GamePhotonManager>();
        audio = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<AudioHandler>();
        if (!photonView.IsMine)
        {
            gameObject.layer = 8;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            float HorizontalAim = Input.GetAxisRaw("Horizontal");
            float VerticalAim = Input.GetAxisRaw("Vertical");
            Vector3 dir = new Vector3(HorizontalAim, VerticalAim, 0).normalized;
            int layermask = ~LayerMask.GetMask("Player");
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dir, Distance, layermask);
            if (inputManager.GetButtonDown("Punch"))
            {
                GetComponent<AnimationHandler>().isAttacking = true;
                GetComponent<AnimationHandler>().photonView.RPC("RPC_PlayAnime", RpcTarget.All, "Punch");
                if (raycastHit2D.collider != null)
                {
                    if (raycastHit2D.transform.tag == "Player")
                    {
                        audio.PlaySound(0, gameObject, 0.05f);
                        if ((GamemodeSingleton.Gamemodes)PhotonNetwork.CurrentRoom.CustomProperties["mode"] == GamemodeSingleton.Gamemodes.Solo ||
                            raycastHit2D.transform.GetComponent<PlayerData>().team != transform.GetComponent<PlayerData>().team && 
                            (GamemodeSingleton.Gamemodes)PhotonNetwork.CurrentRoom.CustomProperties["mode"]==GamemodeSingleton.Gamemodes.Team
                            ){
                            if (GPM.gameStarted) {
                                photonView.RPC("RPC_CallKnockback", raycastHit2D.transform.GetComponent<PhotonView>().Controller, raycastHit2D.transform.GetComponent<PhotonView>().ViewID, 1f);
                            }
                        }
                    }
                }
            }
            Debug.DrawRay(transform.position, dir, Color.red);
        }
    }

    [PunRPC]
    private void RPC_CallKnockback(int id, float DmgMult = 1)
    {
        PhotonView.Find(id).GetComponent<PlayerMovement2D>().Knockback(transform.position, DmgMult);
    }
}
