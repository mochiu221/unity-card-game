using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { set; get; }

    [Header("Game Start Preview Characters")]
    public GameObject beforeStartCharPreview;
    public TextMeshProUGUI battleSlogan;
    public GameObject myTeamCharImagePanel;
    public GameObject enemyTeamCharImagePanel;
    public Image myTeamCharImage;
    public Image enemyTeamCharImage;

    [Header("Preview Card")]
    public GameObject previewCardPanel;
    public TextMeshProUGUI previewCardName;
    public Image previewCardIllustration;
    public TextMeshProUGUI previewCardDescription;
    public TextMeshProUGUI previewCardCreatureType;
    public TextMeshProUGUI previewCardCreatureRace;
    private GameObject previewingCard;
    private static bool previewsAllowed = true;
    public static bool PreviewsAllowed
    {
        get { return previewsAllowed;}

        set 
        { 
            previewsAllowed = value;
        }
    }
    private Sequence sequence;
    public float previewingCardScale = 1.1f;
    public float previewingCardPosY = 0.4f;

    [Header("Dragging card")]
    private bool isDraggingCard = false;
    public bool IsDraggingCard
    {
        get { return isDraggingCard; }
        set
        {
            isDraggingCard = value;
            if (value)
            {
                TurnManager.Instance.whoseTurn.HighlightPlayableCards(true);
            }
            else
            {
                if (!isSelectingTile)
                {
                    TurnManager.Instance.whoseTurn.HighlightPlayableCards();
                }
            }
            
        }
    }

    // Selecting targets
    [Header("Selecting Tiles")]
    private CardDraggingActions selectingTileCardDA;
    public CardDraggingActions SelectingTileCardDA
    {
        get { return selectingTileCardDA; }
        set
        {
            selectingTileCardDA = value;
            if (selectingTileCardDA != null)
            {
                isSelectingTile = true;
                StartSelectTile();
            }
            else
            {
                StopSelectTile();
                isSelectingTile = false;
            }
        }
    }
    private bool isSelectingTile = false;
    public bool IsSelectingTile
    {
        get { return isSelectingTile; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update() 
    {
        // TODO: Optimize
        if (previewingCard != null && !PreviewsAllowed)
        {
            StopPreviewCard();
        }
    }

    // Preview a card
    public void StartPreviewCard(GameObject card)
    {
        OneCardManager cardManager =  card.GetComponent<OneCardManager>();
        OneCreatureManager creatureManager = card.GetComponent<OneCreatureManager>();
        if (card != previewingCard)
        {
            // Update previewing card
            if (previewingCard != null)
            {
                sequence.Kill();
                OneCardManager previewingCardManager = previewingCard.GetComponent<OneCardManager>();
                OneCreatureManager previewingCardCreatureManager = previewingCard.GetComponent<OneCreatureManager>();
                if (previewingCardManager != null)
                {
                    previewingCardManager.transform.localScale = Vector3.one;
                    previewingCardManager.transform.localPosition = new Vector3(previewingCardManager.transform.localPosition.x, 0, previewingCardManager.transform.localPosition.z);
                    previewingCardManager.SelectedNow = false;
                }
                else
                {
                    previewingCardCreatureManager.SelectedNow = false;
                }
            }
            previewingCard = card;
            
            // Get card data
            CardAsset cardAsset = null;
            CharacterAsset charAsset = null;
            string name;
            Sprite img;
            string des;
            string creatureTypeText = "";
            string creatureRaceText = "";
            if (cardManager != null)
            {
                cardAsset = cardManager.cardAsset;
                name = cardAsset.cardName;
                img = cardAsset.previewCardIllustration;
                des = cardAsset.description;
                cardManager.SelectedNow = true;

                if (cardAsset.cardType == CardType.Creature)
                {
                    creatureTypeText = GetCreatureTypeText(cardAsset.creatureType);
                    creatureRaceText = GetCreatureRaceText(cardAsset.creatureRace);
                }

                sequence = DOTween.Sequence();
                sequence.Append(cardManager.transform.DOScale(previewingCardScale, 1f).SetEase(Ease.OutQuint));
                sequence.Join(cardManager.transform.DOLocalMoveY(previewingCardPosY, 1f).SetEase(Ease.OutQuint));
            }
            else
            {
                if (creatureManager.creatrueType != CreatureType.King)
                {
                    cardAsset = creatureManager.cardAsset;
                    name = cardAsset.cardName;
                    img = cardAsset.previewCardIllustration;
                    des = cardAsset.description;
                    creatureManager.SelectedNow = true;

                    if (cardAsset.cardType == CardType.Creature)
                    {
                        creatureTypeText = GetCreatureTypeText(cardAsset.creatureType);
                        creatureRaceText = GetCreatureRaceText(cardAsset.creatureRace);
                    }
                }
                else
                {
                    charAsset = creatureManager.charAsset;
                    name = charAsset.charName;
                    img = charAsset.previewCharIllustration;
                    des = charAsset.description;
                    creatureManager.SelectedNow = true;
                    
                    creatureTypeText = GetCreatureTypeText(CreatureType.King);
                }
            }
            
            // Update preview panel
            previewCardName.text = name;
            previewCardIllustration.sprite = img;
            previewCardDescription.text = des;
            previewCardCreatureType.text = creatureTypeText;
            previewCardCreatureRace.text = creatureRaceText;

            // Show preview
            previewCardPanel.SetActive(true);
        }

    }

    private string GetCreatureTypeText(CreatureType ct)
    {
        string creatureTypeText = "";
        switch (ct)
        {
            case CreatureType.Lost:
                creatureTypeText = "迷失";
                break;
            case CreatureType.Soldier:
                creatureTypeText = "士兵";
                break;
            case CreatureType.Striker:
                creatureTypeText = "強襲";
                break;
            case CreatureType.Archer:
                creatureTypeText = "射手";
                break;
            case CreatureType.Assassin:
                creatureTypeText = "刺客";
                break;
            case CreatureType.Throne:
                creatureTypeText = "王座";
                break;
            case CreatureType.King:
                creatureTypeText = "王";
                break;
            default:
                break;
        }
        creatureTypeText = creatureTypeText != "" ? "位階: " + creatureTypeText : "";
        return creatureTypeText;
    }

    private string GetCreatureRaceText(CreatureRace cr)
    {
        string creatrueRaceText = "";
        switch (cr)
        {
            case CreatureRace.None:
                creatrueRaceText = "";
                break;
            case CreatureRace.FlowerFairy:
                creatrueRaceText = "花靈";
                break;
            default:
                break;
        }
        creatrueRaceText = creatrueRaceText != "" ? "種族: " + creatrueRaceText : "";
        return creatrueRaceText;
    }

    public void StopPreviewCard()
    {
        // Reset previewing card
        if (previewingCard != null)
        {

            sequence.Kill();
            OneCardManager previewingCardManager = previewingCard.GetComponent<OneCardManager>();
            OneCreatureManager previewingCardCreatureManager = previewingCard.GetComponent<OneCreatureManager>();
            if (previewingCardManager != null)
            {
                previewingCardManager.transform.localScale = Vector3.one;
                previewingCardManager.transform.localPosition = new Vector3(previewingCardManager.transform.localPosition.x, 0, previewingCardManager.transform.localPosition.z);
                previewingCardManager.SelectedNow = false;
            }
            else
            {
                previewingCardCreatureManager.SelectedNow = false;
            }
            previewingCard = null;
            
            // Hide preview panel
            previewCardPanel.SetActive(false);
        }
    }

    // Select target
    public void StartSelectTile()
    {
        PreviewsAllowed = false;
        TurnManager.Instance.whoseTurn.HighlightPlayableCards(true);
    }

    public void StopSelectTile()
    {
        PreviewsAllowed = true;
        TurnManager.Instance.whoseTurn.HighlightPlayableCards();
    }

}
