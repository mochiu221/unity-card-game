using UnityEngine;
using System.Collections;

public class UpdateCreatureStatusCommand : Command {

    private int targetID;
    private CreatureStatus statusType;
    private bool status;

    public UpdateCreatureStatusCommand( int targetID, CreatureStatus statusType, bool status)
    {
        this.targetID = targetID;
        this.statusType = statusType;
        this.status = status;
    }

    public override void StartCommandExecution()
    {
        GameObject target = IDHolder.GetGameObjectWithID(targetID);

        switch (statusType)
        {
            case CreatureStatus.Hiding:
                target.GetComponent<OneCreatureManager>().Hiding = status;
                break;

            case CreatureStatus.Allure:
                target.GetComponent<OneCreatureManager>().Allure = status;
                break;

            default:
                break;
        }
        
        CommandExecutionComplete();
    }
}