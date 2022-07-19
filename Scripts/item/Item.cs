using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Item : ScriptableObject
{

    public string Name;
    public string description;
    public int Uses;

    public ItemBooster[] Boost;

    public ItemDoragon[] Vulcano;

    public ItemHalcyon[] Tsunami;

    public Platform[] BreakPlatform;

    public ItemLior[] Tornado;

    public ItemAlskar[] Icicle;

    public screenShake[] Earthquake;

    public ItemHeal[] Heal;

    public Sprite Visual;
}