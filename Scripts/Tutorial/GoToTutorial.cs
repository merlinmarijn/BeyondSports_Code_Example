using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GoToTutorial : MonoBehaviour
{
    public void EnterTutorial()
    {
        SceneManager.LoadScene("-1_Tutorial");
    }
}
