using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardLogic: IIdentifiable
{
    // STATIC (for managing IDs)
    public static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();

    public Player owner;
    private int uniqueCardID; 

    public CardAsset cardAsset;

    private int baseManaCost;
    public int BaseManaCost{ get { return baseManaCost; } }
    public SpellEffect effect;

    public int ID
    {
        get{ return uniqueCardID; }
    }

    public int CurrentManaCost{ get; set; }

    public bool CanBePlayed
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            
            bool validToPlayCreature = true;
            bool vaildToUseSpell = true;
            
            if (cardAsset.cardType == CardType.Creature)
                validToPlayCreature = (ChessboardManager.Instance.GetValidTilesToPlaceCreature(this).Count > 0);
            else if (cardAsset.cardType == CardType.Spell)
                vaildToUseSpell = ChessboardManager.Instance.VaildToUseSpell(this);

            return ownersTurn && validToPlayCreature && vaildToUseSpell;
        }
    }

    public CardLogic(CardAsset cardAsset)
    {
        this.cardAsset = cardAsset;
        uniqueCardID = IDFactory.GetUniqueID();
        baseManaCost = cardAsset.manaCost;
        ResetManaCost();
        if (cardAsset.spellScriptName != null && cardAsset.spellScriptName != "")
        {
            effect = System.Activator.CreateInstance(System.Type.GetType(cardAsset.spellScriptName)) as SpellEffect;
        }
        CardsCreatedThisGame.Add(uniqueCardID, this);
    }

    public void ResetManaCost()
    {
        CurrentManaCost = baseManaCost;
    }
}