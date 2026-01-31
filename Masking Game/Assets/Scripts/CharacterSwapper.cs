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
    Lorien,
    Astelle
}

[Serializable]
public struct Character
{
    public TMP_FontAsset fontAsset;
    public Sprite CharacterPoitrait;
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
    Character Astelle;

    [SerializeField]
    TextMeshProUGUI LineText;
    [SerializeField]
    Image CharacterPoitraitContainer;

    private Animator characterAnimator;

    TMP_FontAsset defaultFont;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultFont = LineText.font;
        characterAnimator = CharacterPoitraitContainer.gameObject.GetComponent<Animator>();
    }

    [YarnCommand("swapCharacter")]
    public void SwapCharacter(string characterName)
    {
        characterName = characterName.ToLower();

        CharacterEnum currentCharacter;

        Enum.TryParse<CharacterEnum>(characterName, true, out currentCharacter);

        if (LineText != null || CharacterPoitraitContainer != null)
        {
            switch (currentCharacter)
            {
                case CharacterEnum.Narrator:
                    LineText.font = Narrator.fontAsset;
                    MoveCharacterBack();
                    break;
                case CharacterEnum.Nacym:
                    LineText.font = Nacym.fontAsset;
                    MoveCharacterBack();
                    break;
                case CharacterEnum.Mum:
                    LineText.font = Mum.fontAsset;
                    MoveCharacterForward(Mum.CharacterPoitrait);
                    break;
                case CharacterEnum.Sulmira:
                    LineText.font = Sulmira.fontAsset;
                    MoveCharacterForward(Sulmira.CharacterPoitrait);
                    break;
                case CharacterEnum.Lorien:
                    LineText.font = Lorien.fontAsset;
                    MoveCharacterForward(Lorien.CharacterPoitrait);
                    break;
                default:
                    LineText.font = defaultFont;
                    break;
            }

        }
    }

    [YarnCommand("hideCharacter")]
    public void HideCharacter()
    {
        CharacterPoitraitContainer.enabled = false;
    }

    void MoveCharacterForward(Sprite characterSprite)
    {
        if(!CharacterPoitraitContainer.enabled) 
        { 
            CharacterPoitraitContainer.enabled=true; 
        }
        if (CharacterPoitraitContainer.sprite != characterSprite) 
        { 
            CharacterPoitraitContainer.sprite = characterSprite;
        }
        
        characterAnimator.Play("MoveForward");
        //CharacterPoitraitContainer.color = Color.white;
    }

    void MoveCharacterBack()
    {
        characterAnimator.Play("MoveBack");
    }
}
