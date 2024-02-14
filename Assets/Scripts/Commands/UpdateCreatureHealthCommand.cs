using UnityEngine;
using System.Collections;

public class UpdateCreatureHealthCommand : Command {

    private int targetID;
    private int amount;
    private int healthAfter;

    public UpdateCreatureHealthCommand( int targetID, int amount, int healthAfter)
    {
        this.targetID = targetID;
        this.amount = amount;
        this.healthAfter = healthAfter;
    }

    public override void StartCommandExecution()
    {
        GameObject target = IDHolder.GetGameObjectWithID(targetID);

        target.GetComponent<OneCreatureManager>().UpdateHealthValue(amount, healthAfter);
        
        CommandExecutionComplete();
    }
}
