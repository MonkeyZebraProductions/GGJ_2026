using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using Yarn.Unity;
using System;

enum CharacterEnum
{
    Narrator,
    Nacym,
    Mum,
    Sulmira,
    Lorien
}

[Serializable]
public struct Character
{
    public TMP_FontAsset fontAsset;
}

public class CharacterSwapper : MonoBehaviour
{
    [SerializeField]
    Character Narrator;
    [SerializeField]
    Character Nacym;
    [SerializeField]
    Character Mum;
    [SerializeField]
    Character Sulmira;
    [SerializeField]
    Character Lorien;

    [SerializeField]
    TextMeshProUGUI LineText;

    TMP_FontAsset defaultFont;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultFont = LineText.font;       
    }

    [YarnCommand("swapCharacter")]
    public void SwapCharacter(string characterName)
    {
        characterName = characterName.ToLower();

        CharacterEnum currentCharacter;

        Enum.TryParse<CharacterEnum>(characterName, true, out currentCharacter);

        switch(currentCharacter)
        {
            case CharacterEnum.Narrator:
                LineText.font = Narrator.fontAsset;
                break;
            case CharacterEnum.Nacym:
                LineText.font = Nacym.fontAsset;
                break;
            case CharacterEnum.Mum:
                LineText.font = Mum.fontAsset;
                break;
            case CharacterEnum.Sulmira:
                LineText.font = Sulmira.fontAsset;
                break;
            case CharacterEnum.Lorien:
                LineText.font = Lorien.fontAsset;
                break;
            default:
                LineText.font = defaultFont;
                break;
        }
    }
}
