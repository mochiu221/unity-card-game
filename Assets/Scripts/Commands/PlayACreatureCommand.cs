using UnityEngine;
using System.Collections;

public class PlayACreatureCommand : Command
{
    private CardLogic cardLogic;
    private Vector2Int tileIndex;
    private Player player;
    private int creatureID;

    public PlayACreatureCommand(CardLogic cardLogic, Player player, Vector2Int tileIndex, int creatureID)
    {
        this.player = player;
        this.cardLogic = cardLogic;
        this.tileIndex = tileIndex;
        this.creatureID = creatureID;
    }

    public override void StartCommandExecution()
    {
        // remove and destroy the card in hand 
        HandManager PlayerHand = player.playerArea.handManager;
        GameObject card = IDHolder.GetGameObjectWithID(cardLogic.ID);
        PlayerHand.RemoveCard(card);
        GameObject.Destroy(card);
        // move this card to the spot 
        ChessboardManager.Instance.AddCreatureAtIndex(tileIndex.x, tileIndex.y, cardLogic.cardAsset, creatureID, player);
    }
}
