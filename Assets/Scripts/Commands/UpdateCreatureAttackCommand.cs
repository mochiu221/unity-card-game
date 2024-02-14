using UnityEngine;
using System.Collections;

public class UpdateCreatureAttackCommand : Command {

    private int targetID;
    private int amount;
    private int attackAfter;

    public UpdateCreatureAttackCommand( int targetID, int amount, int attackAfter)
    {
        this.targetID = targetID;
        this.amount = amount;
        this.attackAfter = attackAfter;
    }

    public override void StartCommandExecution()
    {
        GameObject target = IDHolder.GetGameObjectWithID(targetID);

        target.GetComponent<OneCreatureManager>().UpdateAttackValue(amount, attackAfter);
        
        CommandExecutionComplete();
    }
}