using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemAlskarHail : MonoBehaviourPun
{
    private Animator iceAnim;
    void Start()
    {
        iceAnim = GetComponent<Animator>();
    }

    IEnumerator TurnOff()
    {
        float time = iceAnim.GetCurrentAnimatorStateInfo(0).length -0.3f;
        Debug.Log(iceAnim.GetCurrentAnimatorStateInfo(0).length + " " + time);

        yield return new WaitForSeconds(time);
            this.gameObject.SetActive(false);
    }

    [SerializeField] private float time = 5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine("TurnOff");
            iceAnim.SetBool("hitPlayer", true);
            
            other.GetComponent<PlayerMovement2D>().StartSlow(time);
            other.GetComponent<PlayerMovement2D>().Knockback(transform.position, 10f);
        }
       //other.GetComponent<PlayerMovement2D>().photonView.RPC("SlowDown", RpcTarget.All);
    }
}
