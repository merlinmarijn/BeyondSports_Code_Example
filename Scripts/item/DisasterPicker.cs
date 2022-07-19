using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DisasterPicker : MonoBehaviourPun
{
    private GameObject disDoragon;
    private GameObject disHalcyon;
    private GameObject disLior;

    private GameObject disAlskar;

    [Tooltip("What map are we playing on?")]
    [SerializeField] public int map;
    private bool pickedMap = false;

    void Start()
    {
        disDoragon = gameObject.transform.GetChild(0).gameObject;
        disHalcyon = gameObject.transform.GetChild(1).gameObject;
        disLior = gameObject.transform.GetChild(2).gameObject;
        disAlskar = gameObject.transform.GetChild(3).gameObject;

        disLior.SetActive(false);
        disAlskar.SetActive(false);
        disDoragon.SetActive(false);
        disHalcyon.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!pickedMap)
        TurnDisastorsOn(map); 

        if (transform.position.y < Camera.main.transform.position.y && !disLior.GetComponent<ItemLior>().activeStorm)
            transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 10);

        if (!disDoragon.GetComponent<ItemDoragon>().startFlowing && !disHalcyon.GetComponent<ItemHalcyon>().startWave && !disLior.GetComponent<ItemLior>().activeStorm && !disAlskar.GetComponent<ItemAlskar>().hailTime)
            transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 10);

        if (transform.position.x != Camera.main.transform.position.x && !disHalcyon.GetComponent<ItemHalcyon>().startWave && !disLior.GetComponent<ItemLior>().activeStorm && !disAlskar.GetComponent<ItemAlskar>().hailTime)
            transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z + 10);
    }

    [PunRPC]
    private void TurnDisastorsOn(int map)
    {
        switch (map)
        {
            case 0: // Unity
                disDoragon.SetActive(true);
                disHalcyon.SetActive(true);
                disLior.SetActive(true);
                disAlskar.SetActive(true);
                break;
            case 1: // Doragon
                disDoragon.SetActive(true);
                break;
            case 2: // Halcyon
                disHalcyon.SetActive(true);
                break;
            case 3: // Lior
                disLior.SetActive(true);
                break;
            case 4: // Alskar
                disAlskar.SetActive(false);
                break;
            default:
                disAlskar.SetActive(false);
                disDoragon.SetActive(false);
                disHalcyon.SetActive(false);
                disLior.SetActive(false);
                break;
        }
        pickedMap = true;
    }
}
