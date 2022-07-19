using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerPlayerItem : MonoBehaviour
{
    private GameItemsHandle Handle;
    private SingleplayerPlayerMovement Player;

    public float DelayBeforeItemPickup = 1;

    public int HeldItem;

    public bool CanPickup; //true als je een item kan oppakken
    private bool UseItem; //gaat naar true als je de item use knop in drukt 

    public Item ItmUse;  //dit is wat de items is die je vast hebt
    private int RemainingItemUses; //hoeveel je de item nog kan gebruiken

    private void Start()
    {
        Handle = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<GameItemsHandle>();

        Player = GetComponent<SingleplayerPlayerMovement>();

        ResetItem();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartPickup();
        }
        UseItem = Input.GetButtonDown("Item");
        if (UseItem && HeldItem != -1)
        {
            ActiveItem();
        }
    }

    public void ActiveItem()
    {
        RemainingItemUses -= 1;

        if (ItmUse.Boost.Length > 0)
        {
            //this item has a boost function so boost
            foreach (ItemBooster ItmBoost in ItmUse.Boost)
            {
                //boost the player up
                //Player.Boost(ItmBoost.BoostAmt);
                Player.Jump(ItmBoost.BoostAmt);
            }
        }

        if (RemainingItemUses <= 0)
        {
            //this item is used up 
            ResetItem();
        }
    }

    public void ResetItem()
    {
        ItmUse = null;
        HeldItem = -1;
        CanPickup = true;
    }

    public void StartPickup()
    {
        StartCoroutine(Pickup());
    }

    public IEnumerator Pickup()
    {
        if (HeldItem == -1 && CanPickup)
        {
            CanPickup = false;
            //play pickup animation

            yield return new WaitForSeconds(DelayBeforeItemPickup);
            //wait time before you can use it 

            //check what item it is 
            //int ItemIndex = 0;
            //voor als wij een random item box willen maken 
            int ItemIndex = Random.Range(0, Handle.AllItems.Length + 1);

            ItmUse = Handle.AllItems[ItemIndex];

            HeldItem = ItemIndex;
            RemainingItemUses = ItmUse.Uses;
        }
    }
}
