using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OneCardManager : MonoBehaviour
{

    public CardAsset cardAsset;
    public PlayerTeam playerTeam;
    public CardType cardType;

    [Header("Text Component References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;

    [Header("GameObject References")]
    public GameObject healthIcon;
    public GameObject attackIcon;

    [Header("Image References")]
    public Image cardFrameImage;
    public Image cardIllustration;
    public GameObject cardFaceGlowImage;
    public GameObject cardFaceSelectedImage;

    // Logic
    private bool selectedNow = false;
    public bool SelectedNow
    {
        get
        {
            return selectedNow;
        }
        set
        {
            selectedNow = value;
            cardFaceSelectedImage.SetActive(value);
        }
    }
    
    private bool canBePlayedNow = false;
    public bool CanBePlayedNow
    {
        get
        {
            return canBePlayedNow;
        }
        set
        {
            canBePlayedNow = value;
            cardFaceGlowImage.SetActive(value);
        }
    }

    private void Awake()
    {
        if (cardAsset != null)
        {
            ReadCardFromAsset();
        }
    }

    public void ReadCardFromAsset()
    {
        // Universal actions for any card
        // 1) Add card name
        nameText.text = cardAsset.cardName;
        // 2) Add card mana cost
        manaCostText.text = cardAsset.manaCost.ToString();
        // 3) Add card illustration
        cardIllustration.sprite = cardAsset.cardIllustration;
        // 4) Set card type
        cardType = cardAsset.cardType;

        // Take actions by card type
        switch (cardType)
        {
            case CardType.Creature:
                // This is a creature
                // 1) Add health
                healthText.text = cardAsset.maxHealth.ToString();
                // 2) Add attack
                attackText.text = cardAsset.attack.ToString();
                break;

            case CardType.Spell:

                break;

            default:
                break;
        }
    }

    public void ChangeCost(int newCost)
    {
        manaCostText.text = newCost.ToString();
        // TODO: highlight cost if is different from the original cost
        if (newCost < cardAsset.manaCost)
        {
            manaCostText.color = new Color32(94, 255, 73, 255);
        }
        else
        {
            manaCostText.color = new Color32(255, 255, 255, 255);
        }
        Command.CommandExecutionComplete();
    }

}
