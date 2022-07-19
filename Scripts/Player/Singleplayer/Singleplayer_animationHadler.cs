using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleplayer_animationHadler : MonoBehaviour
{
    private Vector2 movement;
    public bool isGrounded = false;
    private Animator animator;
    [HideInInspector]
    public Vector2 attack;
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
        
        movement = new Vector2(Input.GetAxis("Horizontal"), 0f);
        RPC_SetVars(movement.x, movement.y, isGrounded);
        Vector2 attack = new Vector2(Input.GetAxisRaw("HorizontalAttack"),0f);

        bool flipped = movement.x < 0;
        bool attackflipped = attack.x < 0;
        float rotation = 0;

        if(!noFlip){
            if (Input.GetAxisRaw("HorizontalAttack") != 0)
            {
                rotation = attackflipped ? 180f : 0f;
                // transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
                RPC_SetRotation(rotation);
            }
            else
            {
                rotation = (flipped ? 180f : 0f);
                // transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
                RPC_SetRotation(rotation);
            }
        }

        if(Park){
            if (Input.GetAxisRaw("HorizontalAttack") != 0)
            {
                rotation = attackflipped ? 0f : 180f;
                RPC_SetRotation(rotation);
            }
            else
            {
                rotation = (flipped ? 0f : 180f);
                RPC_SetRotation(rotation);
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

    private void FixedUpdate(){

    }


   
    private void RPC_SetRotation(float rotation)
    {
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));
    }


    private void RPC_PlayAnime(string animeName){
        Debug.Log("test");
        animator.Play(animeName);
    }

    private void RPC_SetVars(float movementX, float movementY, bool isgrounded){

        animator.SetBool("grounded", isgrounded);
        animator.SetFloat("Speed", Mathf.Abs(new Vector2(movementX, movementY).magnitude));
    }

    private void RPC_Animetrigger(string triggedAnime){
        animator.SetTrigger(triggedAnime);
    }

}