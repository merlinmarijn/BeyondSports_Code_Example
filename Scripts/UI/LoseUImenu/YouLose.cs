using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouLose : MonoBehaviour
{
    public void Spectate()
    {
        gameObject.SetActive(false);
        //ik denk dat we hier gewoon de UI uit kunnen zetten, zodat de speler meekijkt met de ander.
    }

    public void GoToMain()
    {
        SceneManager.LoadScene("0_Main");
        //Hierin whatever de main menu scene wordt genoemd.
    }
}
