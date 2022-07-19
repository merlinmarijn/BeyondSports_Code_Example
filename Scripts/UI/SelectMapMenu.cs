using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMapMenu : MonoBehaviour
{
    public void CloseMenu()
    {
        Debug.Log("Quit game");
        Application.Quit();
        //Close screen on close button
    }
}
