using UnityEngine;
using System.Collections;

public class PlayASpellCardCommand: Command
{
    private CardLogic cardLogic;
    private Player player;
    //private ICharacter target;

    public PlayASpellCardCommand(Player player, CardLogic cardLogic)
    {
        this.cardLogic = cardLogic;
        this.player = player;
    }

    public override void StartCommandExecution()
    {
        // move this card to the spot
        player.playerArea.handManager.PlayASpellFromHand(cardLogic.ID);
        // do all the visual stuff (for each spell separately????)
    }
}