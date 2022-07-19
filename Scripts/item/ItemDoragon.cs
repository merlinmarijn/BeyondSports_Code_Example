using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemDoragon : MonoBehaviourPun
{
    [Tooltip("The percentage of bottom screen covered by lava, from bottom to middle")]
    [SerializeField] private int percentage = 60;
    private float stopPosition;

    [Tooltip("The particle system that throws particles around looking like lava")]
    [SerializeField] private ParticleSystem lavaEffect;
    [Tooltip("Is the lava flowing up?")]
    [SerializeField] public bool startFlowing = false;
    [Tooltip("The speed the lava rises up")]
    [SerializeField] private float moveSpeed = 0.05f;
    [Tooltip("The time in seconds for how long the lava stays up")]
    [SerializeField] private int timeRemove = 10;
    private Vector3 originalPos;
    private SpriteRenderer lavaSprite;
    private BoxCollider2D lavaBox;

    // Start is called before the first frame update
    void Start()
    {
        stopPosition = (( (transform.localPosition.y + transform.parent.position.y) / 100) * (percentage) );
        originalPos = transform.localPosition;
        lavaSprite = GetComponent<SpriteRenderer>();
        lavaBox = GetComponent<BoxCollider2D>();
        lavaBox.enabled = false;
        lavaSprite.enabled = false;
    }

    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Space))
             DoragonErupt();

    if (Input.GetKeyDown(KeyCode.P)) 
    Debug.Log("StartFlowing: " + startFlowing + " lavabox: " + lavaBox.enabled + " lavasprite: " + lavaSprite.enabled);

    }

    [PunRPC]
    public void DoragonVar(bool startBool) 
    {
        startFlowing = startBool;
        Camera.main.GetComponent<screenShake>().start = startBool;
        lavaBox.enabled = startBool;
        lavaSprite.enabled = startBool;
        LavaEffect(startBool);
        StartCoroutine(RemoveLava(timeRemove));
    }
    
    [PunRPC]
    public void DoragonErupt()
    {
            if (!startFlowing)
            {
                startFlowing = !startFlowing;
                photonView.RPC("DoragonVar", RpcTarget.All, startFlowing);
            }
            else
                startFlowing = !startFlowing;
    }

    IEnumerator RemoveLava(int time)
    {
        yield return new WaitForSeconds(time);
        if (lavaSprite.enabled && startFlowing)
        photonView.RPC("DoragonErupt", RpcTarget.All);
    }

    void FixedUpdate()
    {
        if (transform.localPosition.y <= stopPosition && startFlowing)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + moveSpeed, transform.localPosition.z);

        if (!startFlowing && transform.localPosition.y >= originalPos.y)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - moveSpeed, transform.localPosition.z);

        if (transform.localPosition.y == originalPos.y && !startFlowing)
        {
            lavaSprite.enabled = false;
            lavaBox.enabled = false;
        }
    }

    void LavaEffect(bool emit)
    {
        if (emit)
        {
            lavaEffect.enableEmission = true;
            lavaEffect.Play();
        }
        if (!emit && lavaEffect.enableEmission)
        {
            lavaEffect.enableEmission = false;
            lavaEffect.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        other.GetComponent<PlayerMovement2D>().Knockback(transform.position, 100);
    }
}
