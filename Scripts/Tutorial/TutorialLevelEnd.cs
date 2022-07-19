using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TutorialLevelEnd : MonoBehaviour
{

    //public void GoToMainScene()
    //{
    //    SceneManager.LoadScene("0_Main");
    //    //gaat naar main scene, tutorial afgelopen
    //}


    public IEnumerator EndTutorial()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player Layer ID: 7
        if (collision.gameObject.layer == 7)
        {
            StartCoroutine(EndTutorial());
        }
    }

}
