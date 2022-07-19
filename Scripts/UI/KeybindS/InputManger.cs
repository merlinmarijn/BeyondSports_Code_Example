using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputManger : MonoBehaviour
{


    public Dictionary<string, KeyCode> buttonKeys;

    private void OnEnable()
    {
        buttonKeys = new Dictionary<string, KeyCode>();


        buttonKeys["Jump"] = KeyCode.W;
        buttonKeys["Item"] = KeyCode.E;
        buttonKeys["Punch"] = KeyCode.F;
        buttonKeys["Left"] = KeyCode.A;
        buttonKeys["Right"] = KeyCode.D;

    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }



    void Start ()
    {
       
    }

    void Update ()
    {

    }

    public void SetButtonForKey(string buttonName, KeyCode keyCode)
    {
        buttonKeys[buttonName] = keyCode;
    }

    public bool GetWalkButtonDown ( string buttonName )
    {
        if (!buttonKeys.ContainsKey( buttonName ) )
        {
            Debug.LogError( "InputManager::GetButtonDown -- No button named: " + buttonName );
            return false;
        }
        return Input.GetKey ( buttonKeys[buttonName] );
    }
    public bool GetButtonDown(string buttonName)
    {
        if (!buttonKeys.ContainsKey(buttonName))
        {
            Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);
            return false;
        }
        return Input.GetKeyDown(buttonKeys[buttonName]);
    }

    /*    public bool GetButtonUp(string buttonName)
        {
            if (!buttonKeys.ContainsKey(buttonName))
            {
                Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);
                return false;
            }
            return Input.GetKeyUp(buttonKeys[buttonName]);
        }
    */
    public string[] GetButtonNames( )
    {
        return buttonKeys.Keys.ToArray();
    }
    
    public string GetKeyName( string buttonName )
    {
        if ( !buttonKeys.ContainsKey(buttonName) )
        {
            Debug.LogError("InputManager::GetButtonDown -- No button named: " + buttonName);
            return "N/A";
        }

        return buttonKeys[buttonName].ToString();
    } 
}
