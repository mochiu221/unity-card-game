using UnityEngine;
using System.Collections;

public class UpdateManaCommand : Command {

    private Player player;
    private int totalMana;
    private int availableMana;

    public UpdateManaCommand(Player player, int totalMana, int availableMana)
    {
        this.player = player;
        this.totalMana = totalMana;
        this.availableMana = availableMana;
    }

    public override void StartCommandExecution()
    {
        player.playerArea.manaManager.TotalMana = totalMana;
        player.playerArea.manaManager.AvailableMana = availableMana;
        CommandExecutionComplete();
    }
}