using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationHandler : MonoBehaviourPunCallbacks
{
    private Vector2 movement;
    public bool isGrounded = false;
    private Animator animator;
    [HideInInspector]
    public Vector2 attack;

    private bool dirRight;
    public bool isAttacking;
    [SerializeField]
    private bool noFlip = false;
    [SerializeField]
    private bool Park = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), 0f);
            photonView.RPC("RPC_SetVars", RpcTarget.All, movement.x, movement.y, isGrounded);
            Vector2 attack = new Vector2(Input.GetAxisRaw("HorizontalAttack"),0f);

            bool flipped = movement.x < 0;
            bool attackflipped = attack.x < 0;
            float rotation = 0;

            if(!noFlip){
                if (Input.GetAxisRaw("HorizontalAttack") != 0)
                {
                    rotation = attackflipped ? 180f : 0f;
                    photonView.RPC("RPC_SetRotation", RpcTarget.All, rotation);
                }
                else
                {
                    rotation = (flipped ? 180f : 0f);
                    photonView.RPC("RPC_SetRotation", RpcTarget.All, rotation);
                }
            }

            if(Park){
                if (Input.GetAxisRaw("HorizontalAttack") != 0)
                {
                    rotation = attackflipped ? 0f : 180f;
                    photonView.RPC("RPC_SetRotation", RpcTarget.All, rotation);
                }
                else
                {
                    rotation = (flipped ? 0f : 180f);
                    photonView.RPC("RPC_SetRotation", RpcTarget.All, rotation);
                }
            }



            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //}
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //}
            if (Input.GetKeyDown(KeyCode.F))
            {

                //photonView.RPC("RPC_PlayAnime", RpcTarget.All, "Punch");
            }
        }
    }

    private void FixedUpdate(){

    }


    [PunRPC]
    private void RPC_SetRotation(float rotation)
    {
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
    }


    [PunRPC]
    public void RPC_PlayAnime(string animeName){
        animator.Play(animeName);
    }

    [PunRPC]
    private void RPC_SetVars(float movementX, float movementY, bool isgrounded){
        animator.SetBool("grounded", isgrounded);
        animator.SetFloat("Speed", Mathf.Abs(new Vector2(movementX, movementY).magnitude));
    }

    [PunRPC]
    public void RPC_Animetrigger(string triggedAnime){
        animator.SetTrigger(triggedAnime);
    }

}
