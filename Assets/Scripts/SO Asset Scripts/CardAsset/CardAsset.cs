using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Creature,
    Spell
}

public enum CreatureType
{
    Lost,
    Soldier,
    Striker,
    Archer,
    Assassin,
    Throne,
    King
}

public enum CreatureRace
{
    None,
    FlowerFairy
}

public enum CreatureStatus
{
    None,
    Hiding,
    Allure
}

public class CardAsset : ScriptableObject
{
    // this object will hold the info about the most general card
    [Header("General Info")]
    // public CharacterAsset characterAsset;  // if this is null, it`s a neutral card
    public string cardName;
    [TextArea(2,3)] public string description;  // Description for spell or character
	public Sprite cardIllustration;
    public Sprite previewCardIllustration;
    public int manaCost;
    public CardType cardType;

    [Header("Creature Info")]
    public Sprite creatureIllustration;
    public CreatureType creatureType;
    public CreatureRace creatureRace;
    public int maxHealth;
    public int attack;
    public int attacksForOneTurn = 1;
    public int resummonCostReduction = 1;
    public string creatureScriptName;
    
    // Special abilities
    public bool invasion;
    public bool hide;
    public bool pioneer;
    public bool allure;

    [Header("Spell Info")]
    public string spellScriptName;

    [Header("Effect target")]
    public int numberOfTarget = 1; // Will be used if not no target

}
