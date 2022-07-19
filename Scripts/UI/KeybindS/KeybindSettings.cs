using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class KeybindSettings : MonoBehaviour
{

    InputManger inputManager;


    [SerializeField] GameObject keyItemPrefab;
    [SerializeField] GameObject keyList;

    
    string buttonToRebind = null;


    Dictionary<string, Text> buttonToLabel;


    // Start is called before the first frame update
    void Start()
    {
        inputManager = FindObjectOfType<InputManger>();

        buttonToLabel = new Dictionary<string, Text>();

        string[] buttonNames = inputManager.GetButtonNames();

        for (int i = 0; i < buttonNames.Length; i++)
        {
            string bn;
            bn = buttonNames[i];

           GameObject go = Instantiate(keyItemPrefab);
            go.transform.SetParent(keyList.transform);
            go.transform.localScale = Vector3.one;

            Text buttonNameText = go.transform.Find("Button Name").GetComponent<Text>();
            buttonNameText.text = bn;
            
            Text keyNameText = go.transform.Find("Button/Key Name").GetComponent<Text>();
            keyNameText.text = inputManager.GetKeyName(bn);
            buttonToLabel[bn] = keyNameText;

            Button keyBindButton = go.transform.Find("Button").GetComponent<Button>();
            keyBindButton.onClick.AddListener( () => { StartRebindFor(bn); });
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (buttonToRebind != null)
        {
            if (Input.anyKeyDown)
            {

                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {

                    if (Input.GetKeyDown(kc))
                    {
                        inputManager.SetButtonForKey(buttonToRebind, kc);
                        buttonToLabel[buttonToRebind].text = kc.ToString();
                        buttonToRebind = null;
                        break;
                    }

                }
            }
        }
    }



    void StartRebindFor (string buttonName)
    {
        Debug.Log("StartRebindFor: " + buttonName);

        buttonToRebind = buttonName;
    }
}
