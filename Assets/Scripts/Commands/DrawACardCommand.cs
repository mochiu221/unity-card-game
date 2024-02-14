using UnityEngine;
using System.Collections;

public class DrawACardCommand : Command {
    // first argument
    // "1" - fast
    // "0" - normal

    private Player player;
    private int handPos;
    private CardLogic cardLogic;
    private bool fast;
    private int ID;
    private bool fromDeck;

    public DrawACardCommand(CardLogic cardLogic, Player player, int positionInHand, bool fast, bool fromDeck)
    {        
        this.cardLogic = cardLogic;
        this.player = player;
        handPos = positionInHand;
        this.fast = fast;
        this.fromDeck = fromDeck;
    }

    public override void StartCommandExecution()
    {
        player.playerArea.deckVisual.CardsInDeck--;
        if (handPos != -1)
        {
            player.playerArea.handManager.GivePlayerACard(cardLogic.cardAsset, cardLogic.ID, fast, fromDeck);
        }
        else
        {
            player.playerArea.handManager.DestroyACard(cardLogic.cardAsset, cardLogic.ID, fromDeck);
        }
    }
}
