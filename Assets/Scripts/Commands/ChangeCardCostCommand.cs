using UnityEngine;
using System.Collections;

public class ChangeCardCostCommand : Command 
{
    private OneCardManager cardManager;
    private int newCost;

    public ChangeCardCostCommand(OneCardManager cardManager, int newCost)
    {
        this.cardManager = cardManager;
        this.newCost = newCost;
    }

    public override void StartCommandExecution()
    {
        cardManager.ChangeCost(newCost);
    }
}