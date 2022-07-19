using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerItem : MonoBehaviour
{
    private GameItemsHandle Handle;
    private PlayerMovement2D Player;
    private PlatformGeneration TPG;

    [Tooltip("How long does the Alruna earthquake take?")]
    [SerializeField] private float durationAlruna = 10f;
    public float DelayBeforeItemPickup = 1;

    [Tooltip("What is the range of the Kafara desert?")]
    [SerializeField] private float disKafRange = 10f;

    public int HeldItem;

    public bool CanPickup; //true als je een item kan oppakken
    private bool UseItem; //gaat naar true als je de item use knop in drukt 

    public Item ItmUse;  //dit is wat de items is die je vast hebt
    private int RemainingItemUses; //hoeveel je de item nog kan gebruiken

    private void Start(){
        Handle = GameObject.FindGameObjectWithTag("ItemHandler").GetComponent<GameItemsHandle>();

        Player = GetComponent<PlayerMovement2D>();
        TPG = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<PlatformGeneration>();
       
        ResetItem();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartPickup();
        }
        UseItem = Input.GetButtonDown("Item");
        if(UseItem && HeldItem != -1){
            ActiveItem();
            GetComponent<AnimationHandler>().photonView.RPC("RPC_PlayAnime", RpcTarget.All, "Use_Item");
        }
    }

    public void ActiveItem(){
        RemainingItemUses -= 1;

        if(ItmUse.Boost.Length > 0){
            //this item has a boost function so boost
            foreach(ItemBooster ItmBoost in ItmUse.Boost){
                //boost the player up
                //Player.Boost(ItmBoost.BoostAmt);
                Player.Jump(ItmBoost.BoostAmt);
            }
        }

        if (ItmUse.BreakPlatform.Length > 0) {
            // collect all colliders within 10 circle radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, disKafRange);
            foreach (Platform ItmBreakPlatform in ItmUse.BreakPlatform)
            {
                // For each platform that we found, run KafaraPlatform
                foreach (Collider2D collider in colliders)
                {
                    if(collider.CompareTag("Platform") && collider != null)
                    collider.transform.GetComponent<Platform>().photonView.RPC("KafaraPlatform", RpcTarget.All);
                }
            }
        }

        if (ItmUse.Icicle.Length > 0){
            //this item brings out the icicles
            foreach (ItemAlskar ItmIcicle in ItmUse.Icicle)
            {
                // activate the icicles
                Handle.transform.GetChild(3).GetComponent<ItemAlskar>().photonView.RPC("StartHailing", RpcTarget.All);
            }
        }

        if(ItmUse.Earthquake.Length > 0){
            //this item brings out the Earthquake
            foreach (screenShake Earthquake in ItmUse.Earthquake)
            {
                // activate the Earthquake
                Camera.main.GetComponent<screenShake>().photonView.RPC("ShakeScreen", RpcTarget.All, durationAlruna);
            }
        }

        if(ItmUse.Vulcano.Length > 0){
            //this item brings out the lava
            foreach (ItemDoragon ItmVulcano in ItmUse.Vulcano)
            {
                // activate the vulcano
                Handle.transform.GetChild(0).GetComponent<ItemDoragon>().photonView.RPC("DoragonErupt", RpcTarget.All);
            }
        }

        if (ItmUse.Tsunami.Length > 0){
            //this item brings out the tsunami
            foreach (ItemHalcyon ItmTsunami in ItmUse.Tsunami)
            {
                // activate the tsunami
                Handle.transform.GetChild(1).GetComponent<ItemHalcyon>().photonView.RPC("DecideSide", RpcTarget.All);
            }
        }

        if (ItmUse.Tornado.Length > 0){
            //this item brings out the tornados
            foreach (ItemLior ItmLior in ItmUse.Tornado)
            {
                // activate the tornados
                Handle.transform.GetChild(2).GetComponent<ItemLior>().photonView.RPC("BlowWind", RpcTarget.All);
            }
        }

        if (ItmUse.Heal.Length > 0){
            //this item has a heal function so heal
            foreach(ItemHeal ItmHeal in ItmUse.Heal){
                //heal the player 
                Player.Heal(ItmHeal.HealAmt);
            }
        }

        if(RemainingItemUses <= 0){
            //this item is used up 
            ResetItem();
        }
    }

    public void ResetItem(){
        ItmUse = null;
        HeldItem = -1;
        CanPickup = true;
    }

    public void StartPickup(){
        StartCoroutine(Pickup());
    }

    public IEnumerator Pickup(){
        if (HeldItem == -1 && CanPickup){
            CanPickup = false;
            //play pickup animation

            yield return new WaitForSeconds(DelayBeforeItemPickup);
            //wait time before you can use it 

            //check what item it is 
            //int ItemIndex = 0;
    
            int ItemIndex = Random.Range(0, Handle.AllItems.Length);

            ItmUse = Handle.AllItems[ItemIndex];

            HeldItem = ItemIndex;
            RemainingItemUses = ItmUse.Uses;
        }
    }
}