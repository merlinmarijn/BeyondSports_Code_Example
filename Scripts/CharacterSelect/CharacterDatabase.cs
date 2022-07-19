using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{
    public Character[] character;

    public int CharacterCount
    {
        get
        {
            return character.Length;
        }
    }

    public Character GetCharacter(int index)
    {
        //return character[index];

        if (index > -1 && index < character.Length)
        {
            return character[index];
        }
        else if (index < 0)
        {
            return character[character.Length-1];
        }
        else if (index > character.Length-1)
        {
            return character[0];
        }
        Debug.Log(index);
        return null;
    }
}
