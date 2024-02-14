using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this class will take all decisions for AI. 

public class AITurnMaker: TurnMaker {

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        // dispay a message that it is enemy`s turn
        new ShowMessageCommand("對方的回合", 2.0f).AddToQueue();
        player.DrawACard();
        StartCoroutine(MakeAITurn());
    }

    // THE LOGIC FOR AI
    IEnumerator MakeAITurn()
    {
        while (MakeOneAIMove())
        {
            yield return null;
        }

        InsertDelay(1f);

        TurnManager.Instance.EndTurn();
    }

    bool MakeOneAIMove()
    {
        if (Command.CardDrawPending())
            return true;
        else
            return PlayACardFromHand();
    }

    bool PlayACardFromHand()
    {
        foreach (CardLogic c in player.hand.CardsInHand)
        {
            if (c.CanBePlayed)
            {
                if (c.cardAsset.cardType == CardType.Creature)
                {
                    // it is a creature card
                    List<Vector2Int> validTilesToPlay = ChessboardManager.Instance.GetValidTilesToPlaceCreature(c);
                    if (validTilesToPlay.Count > 0)
                    {
                        Vector2Int tileIndexToPlay = validTilesToPlay[Random.Range(0, validTilesToPlay.Count)];
                        player.PlayACreatureFromHand(c, tileIndexToPlay, null); // TODO: add Target list
                        InsertDelay(1.5f);
                        return true;
                    }
                }

            }
        }
        return false;
    }

    void InsertDelay(float delay)
    {
        new DelayCommand(delay).AddToQueue();
    }
/*

    bool MakeOneAIMove(bool attackFirst)
    {
        if (Command.CardDrawPending())
            return true;
        else if (attackFirst)
            return AttackWithACreature() || PlayACardFromHand() || UseHeroPower();
        else 
            return PlayACardFromHand() || AttackWithACreature() || UseHeroPower();
    }

    bool PlayACardFromHand()
    {
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.CanBePlayed)
            {
                if (c.ca.MaxHealth == 0)
                {
                    // code to play a spell from hand
                    // TODO: depending on the targeting options, select a random target.
                    if (c.ca.Targets == TargetingOptions.NoTarget)
                    {
                        p.PlayASpellFromHand(c, null);
                        InsertDelay(1.5f);
                        //Debug.Log("Card: " + c.ca.name + " can be played");
                        return true;
                    }                        
                }
                else
                {
                    // it is a creature card
                    p.PlayACreatureFromHand(c, 0);
                    InsertDelay(1.5f);
                    return true;
                }

            }
            //Debug.Log("Card: " + c.ca.name + " can NOT be played");
        }
        return false;
    }

    bool UseHeroPower()
    {
        if (p.ManaLeft >= 2 && !p.usedHeroPowerThisTurn)
        {
            // use HP
            p.UseHeroPower();
            InsertDelay(1.5f);
            //Debug.Log("AI used hero power");
            return true;
        }
        return false;
    }

    bool AttackWithACreature()
    {
        foreach (CreatureLogic cl in p.table.CreaturesOnTable)
        {
            if (cl.AttacksLeftThisTurn > 0)
            {
                // attack a random target with a creature
                if (p.otherPlayer.table.CreaturesOnTable.Count > 0)
                {
                    int index = Random.Range(0, p.otherPlayer.table.CreaturesOnTable.Count);
                    CreatureLogic targetCreature = p.otherPlayer.table.CreaturesOnTable[index];
                    cl.AttackCreature(targetCreature);
                }                    
                else
                    cl.GoFace();
                
                InsertDelay(1f);
                //Debug.Log("AI attacked with creature");
                return true;
            }
        }
        return false;
    }

    void InsertDelay(float delay)
    {
        new DelayCommand(delay).AddToQueue();
    }
*/
}
