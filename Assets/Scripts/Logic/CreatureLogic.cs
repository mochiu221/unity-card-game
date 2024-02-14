using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureLogic: ICharacter 
{
    // STATIC For managing IDs
    public static Dictionary<int, CreatureLogic> CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();

    // PUBLIC FIELDS
    public Player owner;
    public CardAsset cardAsset;
    public CharacterAsset charAsset;
    private bool isKing;
    public bool IsKing
    {
        get{ return isKing; }
    }
    public CreatureEffect effect;
    public int uniqueCreatureID;
    public int ID
    {
        get{ return uniqueCreatureID; }
    }
    public bool frozen = false;

    public bool hiding = false;
    public bool allure = false;

    // the basic health that we have in CardAsset
    private int baseHealth;
    // health with all the current buffs taken into account
    private int maxHealth;
    public int MaxHealth
    {
        get{ return maxHealth;}

        set
        {
            if (value > 0)
            {
                maxHealth = value;
                if (value < Health)
                {
                    Health = value;
                }
            }
            else
            {
                maxHealth = 0;
                Health = value;
            }
        }
    }
        
    private int health;

    public int Health
    {
        get{ return health; }

        set
        {
            if (value > MaxHealth)
                health = baseHealth;
            else if (value <= 0)
                Die();
            else
                health = value;
        }
    }

    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && !frozen);
        }
    }

    private int baseAttack;
    // attack with buffs
    private int attack;
    public int Attack
    {
        get{ return attack; }
        set
        {
            if (value > 0)
                attack = value;
            else
                attack = 0;
        }
    }
        
    private int attacksForOneTurn = 1;
    public int AttacksLeftThisTurn
    {
        get;
        set;
    }

    private Vector2Int tilePos;
    public Vector2Int TilePos
    {
        get{ return tilePos; }
        set
        {
            if ( AttacksLeftThisTurn > 0)
            {
                tilePos = value;
                AttacksLeftThisTurn--;
            }
        }
    }

    private int baseResummonCostReduction;
    public int BaseResummonCostReduction
    {
        get { return baseResummonCostReduction; } 
    }
    public int resummonCostReduction;
    
    // CONSTRUCTOR of normal creature
    public CreatureLogic(Player owner, CardAsset cardAsset, Vector2Int tilePos)
    {
        this.cardAsset = cardAsset;
        baseHealth = cardAsset.maxHealth;
        maxHealth = baseHealth;
        Health = baseHealth;
        baseAttack = cardAsset.attack;
        attack = baseAttack;
        this.tilePos = tilePos;
        this.isKing = false;
        hiding = cardAsset.hide;
        allure = cardAsset.allure;
        // Resummon cost reduction
        baseResummonCostReduction = cardAsset.resummonCostReduction;
        resummonCostReduction = baseResummonCostReduction;
        
        attacksForOneTurn = cardAsset.attacksForOneTurn;
        
        // AttacksLeftThisTurn is now equal to 0
        if (cardAsset.pioneer)
            AttacksLeftThisTurn = attacksForOneTurn;
        
        this.owner = owner;
        uniqueCreatureID = IDFactory.GetUniqueID();
        if (cardAsset.creatureScriptName!= null && cardAsset.creatureScriptName!= "")
        {
            effect = System.Activator.CreateInstance(System.Type.GetType(cardAsset.creatureScriptName), new System.Object[]{owner, this}) as CreatureEffect;
            effect.RegisterEffect();
        }
        CreaturesCreatedThisGame.Add(uniqueCreatureID, this);
    }

    // CONSTRUCTOR of king
    public CreatureLogic(Player owner, CharacterAsset charAsset, Vector2Int tilePos)
    {
        this.charAsset = charAsset;
        baseHealth = charAsset.maxHealth;
        maxHealth = baseHealth;
        Health = baseHealth;
        baseAttack = charAsset.attack;
        attack = baseAttack;
        this.tilePos = tilePos;
        this.isKing = true;
        this.owner = owner;
        uniqueCreatureID = IDFactory.GetUniqueID();
        CreaturesCreatedThisGame.Add(uniqueCreatureID, this);
    }

    public void OnTurnStart()
    {
        AttacksLeftThisTurn = attacksForOneTurn;
        // if (TurnManager.Instance.turnCount <= 1)
        // {
        //     AttacksLeftThisTurn = 0;
        // }
    }

    public void Die()
    {   
        
        ChessboardManager.Instance.chessboard.creaturesOnTile[tilePos.x, tilePos.y] = null;

        if (IsKing)
        {
            GameManager.Instance.GameOver(owner);
        }

        new CreatureDieCommand(tilePos).AddToQueue();
        
    }

    public void AttackCreature (CreatureLogic target)
    {
        AttacksLeftThisTurn--;
        // calculate the values so that the creature does not fire the DIE command before the Attack command is sent
        int targetHealthAfter = target.Health - Attack;
        int attackerHealthAfter = Health;

        new CreatureAttackCommand(target.uniqueCreatureID, uniqueCreatureID, target.Attack, Attack, attackerHealthAfter, targetHealthAfter).AddToQueue();

        if (target.effect != null)
        {
            target.effect.DefenseEffect(this);
        }

        target.Health -= Attack;
        // Health -= target.Attack;
        
    }

    public void AttackCreatureWithID(int uniqueCreatureID)
    {
        CreatureLogic target = CreatureLogic.CreaturesCreatedThisGame[uniqueCreatureID];
        AttackCreature(target);
    }

    public void ChangeHidingStatus(bool hide)
    {
        if (hiding != hide)
        {
            hiding = hide;
            new UpdateCreatureStatusCommand(uniqueCreatureID, CreatureStatus.Hiding, hide).AddToQueue();
        }
    }

    public void ChangeAllureStatus(bool allure)
    {
        if (this.allure != allure)
        {
            this.allure = allure;
            new UpdateCreatureStatusCommand(uniqueCreatureID, CreatureStatus.Allure, allure).AddToQueue();
        }
    }

    public List<Vector2Int> GetValidTilesToAttackOrMove()
    {
        return ChessboardManager.Instance.GetValidTilesToAttackOrMove(TilePos, cardAsset.creatureType, owner.playerTeam);
    }

    public List<Vector2Int> GetAttackRange()
    {
        return ChessboardManager.Instance.GetAttackRange(TilePos, cardAsset.creatureType, owner.playerTeam);
    }

    public TileAsset GetTileAsset()
    {
        return ChessboardManager.Instance.chessboard.GetTileAssetByIndex(TilePos);
    }

}
