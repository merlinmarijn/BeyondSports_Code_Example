using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnjailPlatform : MonoBehaviour
{
    private int DefaultTime = 3;
    public float currentTime;
    public bool CanUnjail = true;

    private void Awake()
    {
        currentTime = DefaultTime;
    }

    //void Unjail(int id)
    //{
    //    GameObject.FindGameObjectWithTag("Jail").GetComponent<JailSystem>().GetUnjailed();
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            //Can put something here when player STARTS standing on unjail platform
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0 && CanUnjail)
            {
                GameObject.FindGameObjectWithTag("Jail").GetComponent<JailSystem>().GetUnjailed((int)collision.gameObject.GetComponent<PlayerData>().team);
                Debug.Log("ALREADY UNJAILED");
                CanUnjail = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        currentTime = DefaultTime;
    }
}
