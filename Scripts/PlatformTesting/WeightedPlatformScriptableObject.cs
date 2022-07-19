using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weighted Platform Config", menuName = "Scriptable Objects/Weighted Platform Config")]
public class WeightedPlatformScriptableObject : ScriptableObject
{
    public Platform platform;
    [Range(0, 1)]
    public float MinWeight;
    [Range(0, 1)]
    public float MaxWeigth;

    public float GetWeight()
    {
        return Random.Range(MinWeight, MaxWeigth);
    }
}
