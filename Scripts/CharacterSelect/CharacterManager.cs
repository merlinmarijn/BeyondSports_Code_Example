using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;

    public List <Text> nameText;
    public List <Image> artworkSprite;

    private int selectedOption = 0;

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedCharacter"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

        UpdateCharacter(selectedOption);
        UpdateSidepanels(selectedOption);
    }

    public void NextOption()
    {
        selectedOption++;

        if (selectedOption >= characterDB.CharacterCount)
        {
            selectedOption = 0;
        }

        UpdateCharacter(selectedOption);
        UpdateSidepanels(selectedOption);
        //Save();
    }

    public void BackOption()
    {
        selectedOption--;

        if (selectedOption < 0)
        {
            selectedOption = characterDB.CharacterCount - 1;
        }

        UpdateCharacter(selectedOption);
        UpdateSidepanels(selectedOption);
        //Save();
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption);
        artworkSprite[1].sprite = character.characterSprite;
        nameText[1].text = character.characterName;
    }

    private void UpdateSidepanels(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption - 1);
        artworkSprite[0].sprite = character.characterSprite;
        nameText[0].text = character.characterName;

        character = characterDB.GetCharacter(selectedOption + 1);
        artworkSprite[2].sprite = character.characterSprite;
        nameText[2].text = character.characterName;
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedCharacter");
    }

    public void Save()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedOption);
    }

}
