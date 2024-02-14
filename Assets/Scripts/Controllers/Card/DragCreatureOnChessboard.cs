using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class DragCreatureOnChessboard : CardDraggingActions {

    private int savedHandSlot;
    private CardVisualController whereIsCard;
    private IDHolder idScript;
    private CardVisualStates tempState;
    private OneCardManager manager;
    private CardLogic cardLogic;
    private int savedOldCost;
    private int currentCost;
    private List<Vector2Int> validTilesToPlay;
    private OneTileManager targetTile;

    // Time to move back to hand
    private float moveBackTime = 0.3f;

    public override bool CanDrag
    {
        get
        { 
            // TODO: include full field check
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
        validTilesToPlay = ChessboardManager.Instance.GetValidTilesToPlaceCreature(cardLogic);
        whereIsCard.VisualState = CardVisualStates.Dragging;
        whereIsCard.BringToFront();
        // Save current cost
        savedOldCost = cardLogic.CurrentManaCost;
        currentCost = savedOldCost;
        // Highlight tiles
        ChessboardManager.Instance.HighlightValidTilesToPlaceCreature(cardLogic);
    }

    public override void OnDraggingInUpdate()
    {
        OneTileManager newTileManagerOnHover = ChessboardManager.Instance.GetCursorOverTile();
        if (tileManagerOnHover == null)
        {
            if (newTileManagerOnHover != null)
            {
                EnterATile(newTileManagerOnHover);
            }
        }
        else
        {
            if (newTileManagerOnHover == null)
            {
                LeaveATile(tileManagerOnHover);
            }
            else if (tileManagerOnHover != newTileManagerOnHover)
            {
                ChangeATile(tileManagerOnHover, newTileManagerOnHover);
            }
        }
        tileManagerOnHover = newTileManagerOnHover;
    }

    private void EnterATile(OneTileManager newTM)
    {
        // Highlight tile
        if (validTilesToPlay.Contains(newTM.thisTileIndex))
            newTM.IsSelected = true;

        CreatureLogic newCrl = ChessboardManager.Instance.chessboard.creaturesOnTile[newTM.thisTileIndex.x, newTM.thisTileIndex.y];
        // Creature effect
        if (newCrl != null && newCrl.effect != null)
            newCrl.effect.OnDraggingBeingResummonedTargetEffect(cardLogic);
        
        // Cost change
        if (newCrl != null && newCrl.owner.playerTeam == TurnManager.Instance.whoseTurn.playerTeam && !newCrl.IsKing && newCrl.resummonCostReduction > 0)
        {
            int newCost = savedOldCost - newCrl.resummonCostReduction;
            newCost = newCost < 0 ? 0 : newCost;
            UpdateCardCost(currentCost, newCost);
        }
        else
        {
            UpdateCardCost(cardLogic.CurrentManaCost, savedOldCost);
        }
    }
    private void LeaveATile(OneTileManager oldTM)
    {
        // Remove highlight
        oldTM.IsSelected = false;

        CreatureLogic oldCrl = ChessboardManager.Instance.chessboard.creaturesOnTile[oldTM.thisTileIndex.x, oldTM.thisTileIndex.y];
        // Remove Creature effect
        if (oldCrl != null && oldCrl.effect != null)
            oldCrl.effect.StopOnDraggingCardEffect(cardLogic);

        // Cost change
        UpdateCardCost(cardLogic.CurrentManaCost, savedOldCost);
    }
    private void ChangeATile(OneTileManager oldTM, OneTileManager newTM)
    {
        LeaveATile(oldTM);
        EnterATile(newTM);
    }
    private void UpdateCardCost(int c1, int c2)
    {
        if (c1 != c2)
        {
            currentCost = c2;
            cardLogic.CurrentManaCost = c2;
            new ChangeCardCostCommand(manager, c2).AddToQueue();
        }
    }


    public override void OnEndDrag()
    {
        // Remove highlight
        ChessboardManager.Instance.RemoveAllTileHighlight();
        // 1) Check if we are holding a card over the table
        if (DragSuccessful())
        {
            // determine chessboard tile position
            targetTile = tileManagerOnHover;
            tileManagerOnHover = null;

            if (cardLogic.effect != null && cardLogic.effect.hasTarget)
            {
                SelectingTargetState();
            }
            else
            {
                // play this card
                playerOwner.PlayACreatureFromHand(cardLogic.ID, targetTile.thisTileIndex, null);
            }
        }
        else
        {
            BackToHand();
        }
    }

    public override void BackToHand()
    {
        if (targetTile != null)
        {
            CreatureLogic oldCrl = ChessboardManager.Instance.chessboard.creaturesOnTile[targetTile.thisTileIndex.x, targetTile.thisTileIndex.y];
            // Remove Creature effect
            if (oldCrl != null && oldCrl.effect != null)
                oldCrl.effect.StopOnDraggingCardEffect(cardLogic);
        }
        
        // Reset cost back
        UpdateCardCost(cardLogic.CurrentManaCost, savedOldCost);

        targetTile = null;
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

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(PlayerHand.selectEffectTargetSpot.position, 0.5f));
        s.Join(transform.DORotate(new Vector3(0f, 360f, 0f), 0.5f, RotateMode.FastBeyond360));
    }

    public override void PlayCardOnTarget()
    {
        ChessboardManager.Instance.RemoveAllTileHighlight();
        List<Vector2Int> indexList = new List<Vector2Int>();
        foreach (var tm in tilesList)
        {
            indexList.Add(tm.thisTileIndex);
        }
        // play this card
        playerOwner.PlayACreatureFromHand(cardLogic.ID, targetTile.thisTileIndex, indexList);
    }

    protected override bool DragSuccessful()
    {
        bool tileCanAddCreature = (tileManagerOnHover != null && ChessboardManager.Instance.VaildTileToPlaceCreature(cardLogic, tileManagerOnHover.thisTileIndex));
        return tileCanAddCreature;
    }

    public override CardLogic GetCardLogic()
    {
        return cardLogic;
    }
}
