using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragCreatureAttackOrMove : CreatureDraggingActions{


    // LineRenderer that is attached to a child game object to draw the arrow
    private LineRenderer lr;

    private OneTileManager targetTile;
    private OneTileManager hoverTile;

    private bool isHighlightingTile = false;

    // when we stop dragging, the gameObject that we were targeting will be stored in this variable.
    private GameObject target;
    private GameObject targetOnImage;

    // Reference to creature manager, attached to the parent game object
    private OneCreatureManager manager;
    private OneTileManager currentTile;

    private List<Vector2Int> validTilesToAttackOrMove;

    void Awake()
    {
        // establish all the connections
        targetOnImage = transform.Find("TargetOnImage").gameObject;
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = "AboveEverything";

        manager = GetComponentInParent<OneCreatureManager>();
    }

    public override bool CanDrag
    {
        get
        {   
            // we can drag this card if 
            // a) we can control this our player (this is checked in base.canDrag)
            // b) creature "CanAttackNow" - this info comes from logic part of our code into each creature`s manager script
            return base.CanDrag && manager.CanAttackNow;
        }
    }

    public override void OnStartDrag()
    {
        currentTile = GetComponentInParent<OneTileManager>();
        // enable line renderer to start drawing the line.
        lr.enabled = true;
        validTilesToAttackOrMove = ChessboardManager.Instance.GetValidTilesToAttackOrMove(currentTile.thisTileIndex, manager.creatrueType, manager.playerTeam);
        
    }

    public override void OnDraggingInUpdate()
    {
        Vector3 notNormalized = transform.position - transform.parent.position;
        Vector3 direction = notNormalized.normalized;
        float distanceToTarget = (direction*0.1f).magnitude;

        if (notNormalized.magnitude > distanceToTarget)
        {
            // Draw a curve between the creature and the target
            // Last point
            Vector3 pos3 = transform.position - direction*0.1f;
            hoverTile = ChessboardManager.Instance.GetCursorOverTile();
            if (hoverTile != null && hoverTile != currentTile && validTilesToAttackOrMove.Contains(hoverTile.thisTileIndex))
            {
                pos3 = hoverTile.transform.position;
                targetOnImage.transform.position = hoverTile.transform.position;
                targetOnImage.SetActive(true);
                targetTile = hoverTile;
            }
            else
            {
                targetOnImage.SetActive(false);
                targetTile = null;
            }
            // First point
            Vector3 pos1 = transform.parent.position;
            Vector3 pos2 = new Vector3((pos1.x + pos3.x)/2, Mathf.Max(pos1.y, pos3.y)+1f, (pos1.z + pos3.z)/2);

            var pointList = new List<Vector3>();
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / 12)
            {
                var tangentLineVertex1 = Vector3.Lerp(pos1, pos2, ratio);
                var tangentLineVertex2 = Vector3.Lerp(pos2, pos3, ratio);
                var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierpoint);
            }
            lr.positionCount = pointList.Count;
            lr.SetPositions(pointList.ToArray());

            lr.enabled = true;

            if (!isHighlightingTile)
            {
                ChessboardManager.Instance.HighlightTilesByIndexes(validTilesToAttackOrMove);
                isHighlightingTile = true;
            }
        }
        else
        {
            // if the target is not far enough from creature, do not show the arrow
            lr.enabled = false;
            targetTile = null;
        }

    }

    public override void OnEndDrag()
    {
        if (targetTile != null)
        {
            Vector2Int targetTileIndex = targetTile.thisTileIndex;
            CreatureLogic target = ChessboardManager.Instance.chessboard.creaturesOnTile[targetTileIndex.x, targetTileIndex.y];
            if (target != null) // Attack
            {
                ChessboardManager.Instance.chessboard.AttackCreature(currentTile.thisTileIndex, targetTile.thisTileIndex);
            }
            else // Move
            {
                ChessboardManager.Instance.chessboard.MoveCreature(currentTile.thisTileIndex, targetTile.thisTileIndex);
            }
            
        }

        // return target and arrow to original position
        transform.localPosition = Vector3.zero;
        targetOnImage.SetActive(false);
        lr.enabled = false;
        hoverTile = null;
        targetTile = null;

        isHighlightingTile = false;
        ChessboardManager.Instance.RemoveAllTileHighlight();
    }

    // NOT USED IN THIS SCRIPT
    protected override bool DragSuccessful()
    {
        return true;
    }
}