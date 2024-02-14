using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class DragSpell: CardDraggingActions{

    private int savedHandSlot;
    private CardVisualController whereIsCard;
    private CardVisualStates tempState;
    private OneCardManager manager;
    private CardLogic cardLogic;
    private int currentCost;

    // Time to move back to hand
    private float moveBackTime = 0.3f;

    public override bool CanDrag
    {
        get
        { 
            // TODO : include full field check
            return base.CanDrag && manager.CanBePlayedNow;
        }
    }

    private void Awake()
    {
        whereIsCard = GetComponent<CardVisualController>();
        manager = GetComponent<OneCardManager>();
    }

    private void Start() 
    {
        cardLogic = CardLogic.CardsCreatedThisGame[GetComponent<IDHolder>().UniqueID];
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;
        tempState = whereIsCard.VisualState;
        whereIsCard.VisualState = CardVisualStates.Dragging;
        whereIsCard.BringToFront();

    }

    public override void OnDraggingInUpdate()
    {
    }

    public override void OnEndDrag()
    {
        // 1) Check if we are holding a card over the table
        if (DragSuccessful())
        {
            if (cardLogic.effect != null && cardLogic.effect.hasTarget)
            {
                // Select target step
                SelectingTargetState();
            }
            else
            {
                // play this card
                playerOwner.PlayASpellFromHand(cardLogic, null);
            }
        }
        else
        {
            BackToHand();
        } 
    }
    public override void BackToHand()
    {
        tilesList.Clear();
        UIManager.Instance.SelectingTileCardDA = null;
        ChessboardManager.Instance.RemoveAllTileHighlight();
        // Set old sorting order 
        whereIsCard.SetHandSortingOrder();
        whereIsCard.VisualState = tempState;
        // Move this card back to its slot position
        HandManager PlayerHand = TurnManager.Instance.whoseTurn.playerArea.handManager;
        Vector3 oldCardPos = PlayerHand.slots.Children[savedHandSlot].transform.localPosition;
        transform.DOLocalMove(oldCardPos, moveBackTime);
    }

    public override void SelectingTargetState()
    {
        UIManager.Instance.SelectingTileCardDA = this;
        UIManager.PreviewsAllowed = false;
        ChessboardManager.Instance.HighlightEffectTargetValidTiles(cardLogic);
        numberOfTarget = manager.cardAsset.numberOfTarget;
        HandManager PlayerHand = TurnManager.Instance.whoseTurn.playerArea.handManager;
        transform.DOMove(PlayerHand.selectEffectTargetSpot.position, 0.5f);
    }

    public override void PlayCardOnTarget()
    {
        ChessboardManager.Instance.RemoveAllTileHighlight();
        List<Vector2Int> tileIndexesList = new List<Vector2Int>();
        foreach (var tm in tilesList)
        {
            tileIndexesList.Add(tm.thisTileIndex);
        }
        // play this card
        playerOwner.PlayASpellFromHand(cardLogic, tileIndexesList);
    }

    protected override bool DragSuccessful()
    {
        return ChessboardManager.Instance.IsCursorOverTable();
    }

    public override CardLogic GetCardLogic()
    {
        return cardLogic;
    }
}
