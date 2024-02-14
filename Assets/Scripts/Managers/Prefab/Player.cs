using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Initialized in PlayersManager")]
    public int playerID;
    public PlayerTeam playerTeam;
    public GameObject king;

    [Header("Visual position")]
    public PlayerArea playerArea;

    [Header("Data")]
    public CharacterAsset charAsset;
    public Deck deck;

    [Header("Logic")]
    public Hand hand;

    private int bonusManaThisTurn = 0;
    public bool usedHeroPowerThisTurn = false;

    public int ID
    {
        get{ return playerID; }
    }

    public PlayerTeam Team
    {
        get{ return playerTeam; }
    }

    private int manaThisTurn;
    public int ManaThisTurn
    {
        get{ return manaThisTurn; }
        set
        {
            manaThisTurn = value;
            //playerArea.manaManager.TotalMana = manaThisTurn;
            new UpdateManaCommand(this, manaThisTurn, manaLeft).AddToQueue();
        }
    }

    private int manaLeft;
    public int ManaLeft
    {
        get
        { return manaLeft;}
        set
        {
            manaLeft = value;
            //playerArea.manaManager.AvailableMana = manaLeft;
            new UpdateManaCommand(this, ManaThisTurn, manaLeft).AddToQueue();
            if (TurnManager.Instance.whoseTurn == this)
                HighlightPlayableCards();
        }
    }

    public Player otherPlayer
    {
        get
        {
            if (PlayersManager.Instance.myPlayer == this)
                return PlayersManager.Instance.enemyPlayer;
            else
                return PlayersManager.Instance.myPlayer;
        }
    }

    public delegate void VoidWithNoArguments();
    //public event VoidWithNoArguments CreaturePlayedEvent;
    //public event VoidWithNoArguments SpellPlayedEvent;
    public event VoidWithNoArguments StartTurnEvent;
    public event VoidWithNoArguments EndTurnEvent;

    public void OnTurnStart()
    {
        // Add one mana
        // Debug.Log("In ONTURNSTART for "+ gameObject.name);
        ManaThisTurn++;
        ManaLeft = ManaThisTurn;
        foreach (CreatureLogic cl in ChessboardManager.Instance.chessboard.creaturesOnTile)
        {
            if (cl != null && cl.owner == this)
            {
                cl.OnTurnStart();
            }
        }
        if(StartTurnEvent != null)
            StartTurnEvent.Invoke();
    }

    public void GetBonusMana(int amount)
    {
        bonusManaThisTurn += amount;
        ManaThisTurn += amount;
        ManaLeft += amount;
    }

    public void OnTurnEnd()
    {
        EndTurnEvent?.Invoke();
        ManaThisTurn -= bonusManaThisTurn;
        bonusManaThisTurn = 0;
        GetComponent<TurnMaker>().StopAllCoroutines();
    }

    // public void UpdateDeckCardsOrder(NetworkList<int> order)
    // {
    //     List<CardAsset> tempDeckCards = new List<CardAsset>();
    //     for (int i = 0; i < deck.cards.Count; i++)
    //     {
    //         CardAsset ca = deck.cards[order[i]];
    //         tempDeckCards.Add(ca);
    //     }
    //     deck.cards = tempDeckCards;

    //     Debug.Log("------ deck ------");
    //     foreach (var ca in deck.cards)
    //     {
    //         Debug.Log(ca.cardName);
    //     }
    // }

    public void DrawACard(bool fast = false)
    {
        if (deck.cards.Count > 0)
        {
            // Get a card from the deck
            CardLogic newCard = new CardLogic(deck.cards[0]);
            newCard.owner = this;
            deck.cards.RemoveAt(0);

            if (hand.CardsInHand.Count < playerArea.handManager.slots.Children.Length)
            {
                // Add to hand
                int indexToPlaceACard = hand.CardsInHand.Count;
                hand.CardsInHand.Add(newCard);
                new DrawACardCommand(newCard, this, indexToPlaceACard, fast, fromDeck: true).AddToQueue(); 
            }
            else
            {
                new DrawACardCommand(newCard, this, -1, false, fromDeck: true).AddToQueue(); 
            }
        }
        else
        {
            // TODO: there are no cards in the deck, take fatigue damage.
        }
       
    }

    public void DrawARandomCardByRace(CreatureRace race)
    {
        int index = -1;
        if (deck.cards.Count > 0)
        {
            // Get the list of all card asset which fulfil the condition
            List<int> tempCardIndexList = new List<int>();
            CardAsset ca;
            for (int i = 0; i < deck.cards.Count; i++)
            {
                ca = deck.cards[i];
                if (ca.cardType == CardType.Creature && ca.creatureRace == race)
                {
                    tempCardIndexList.Add(i);
                }
            }

            if (tempCardIndexList.Count > 0)
            {
                int rnd = Random.Range(0,tempCardIndexList.Count);
                index = tempCardIndexList[rnd];
            }
        }

        GameModeMultiplayerManager.Instance.DrawACardAtIndex(index, ID);
    }

    public void DrawACardAtIndex(int index)
    {
        if (deck.cards.Count > 0 )
        {
            CardLogic newCard = null;

            if (index >= 0 && index < deck.cards.Count)
            {
                newCard = new CardLogic(deck.cards[index]);
                newCard.owner = this;
                deck.cards.RemoveAt(index);
            }

            if (newCard != null)
            {
                if (hand.CardsInHand.Count < playerArea.handManager.slots.Children.Length)
                {
                    // Add to hand
                    int indexToPlaceACard = hand.CardsInHand.Count;
                    hand.CardsInHand.Add(newCard);
                    new DrawACardCommand(newCard, this, indexToPlaceACard, false, fromDeck: true).AddToQueue(); 
                }
                else
                {
                    new DrawACardCommand(newCard, this, -1, false, fromDeck: true).AddToQueue(); 
                }
            }
        }
        else
        {
            // TODO: there are no cards in the deck, take fatigue damage.
        }
    }


    // Play a spell from hand
    public void PlayASpellFromHand(CardLogic playedCard, List<Vector2Int> targetList)
    {
          PlayASpellFromHand(playedCard.ID, targetList);
    }
    
    public void PlayASpellFromHand(int UniqueID, List<Vector2Int> targetList)
    {
        int targetInt = EncodeTargetListToInt(targetList);

        GameModeMultiplayerManager.Instance.PlayASpellFromHand(UniqueID, targetInt, ID);
    }

    public void PlayASpellFromHandAction(int UniqueID, int targetInt)
    {
        CardLogic playedCard = CardLogic.CardsCreatedThisGame[UniqueID];

        ManaLeft -= playedCard.CurrentManaCost;
        // Spell effect
        if (playedCard.effect != null)
        {
            List<Vector2Int> targetList = DecodeIntToTargetList(targetInt);
            playedCard.effect.ActivateEffect(targetList);
        }
        
        new PlayASpellCardCommand(this, playedCard).AddToQueue();
        // remove this card from hand
        hand.CardsInHand.Remove(playedCard);
    }


    // Play a creature from hand
    public void PlayACreatureFromHand(CardLogic playedCard, Vector2Int tileIndex, List<Vector2Int> targetList)
    {
        PlayACreatureFromHand(playedCard.ID, tileIndex, targetList);
    }
    public void PlayACreatureFromHand(int UniqueID, Vector2Int tileIndex, List<Vector2Int> targetList)
    {
        int targetInt = EncodeTargetListToInt(targetList);

        GameModeMultiplayerManager.Instance.PlayACreatureFromHand(UniqueID, tileIndex, targetInt, ID);
    }

    public void PlayACreatureFromHandAction(int UniqueID, Vector2Int tileIndex, int targetInt)
    {
        CardLogic playedCard = CardLogic.CardsCreatedThisGame[UniqueID];
        
        ManaLeft -= playedCard.CurrentManaCost;
        // create a new creature object and add it to Table
        CreatureLogic newCreature = new CreatureLogic(this, playedCard.cardAsset, tileIndex);
        CreatureLogic oldCreature = ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y];
        if (oldCreature != null)
        {
            oldCreature.Die();
        }
        ChessboardManager.Instance.chessboard.creaturesOnTile[tileIndex.x, tileIndex.y] = newCreature;
        
        new PlayACreatureCommand(playedCard, this, tileIndex, newCreature.uniqueCreatureID).AddToQueue();
        // remove this card from hand
        hand.CardsInHand.Remove(playedCard);
        // Summon effect
        if (newCreature.effect != null)
        {
            List<Vector2Int> targetList = DecodeIntToTargetList(targetInt);
            newCreature.effect.SummonEffect(targetList);
        }
        HighlightPlayableCards();
    }


    // Encode and Decode target list
    public int EncodeTargetListToInt(List<Vector2Int> targetList)
    {
        // from left to right, each single number represent a tile:
        // 1A, 1B, 1C, 2A, ... 3C
        // 1 = not the target, 2 = target
        int targetInt = 111111111;

        if (targetList == null || targetList.Count == 0) return targetInt;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (targetList.Contains(new Vector2Int(x,y)))
                {
                    targetInt += (int) Mathf.Pow(10, (2-y)*3+(2-x));
                }
            }
        }
        return targetInt;
    }
    public List<Vector2Int> DecodeIntToTargetList(int targetInt)
    {
        List<Vector2Int> targetList = new List<Vector2Int>();
        string targetIntStr = targetInt.ToString();
        for (int i = 0; i < targetIntStr.Length; i++)
        {
            if (targetIntStr[i] == '2')
            {
                int x = i % 3;
                int y = i / 3;
                targetList.Add(new Vector2Int(x,y));
            }
        }
        return targetList;
    }

    public void Die()
    {
        /*
        // game over
        // block both players from taking new moves 
        playerArea.ControlsON = false;
        otherPlayer.playerArea.ControlsON = false;
        TurnManager.Instance.StopTheTimer();
        new GameOverCommand(this).AddToQueue();
        */
    }

    // METHODS TO SHOW GLOW HIGHLIGHTS
    public void HighlightPlayableCards(bool removeAllHighlights = false)
    {
        //Debug.Log("HighlightPlayable remove: "+ removeAllHighlights);
        foreach (CardLogic cl in hand.CardsInHand)
        {
            GameObject g = IDHolder.GetGameObjectWithID(cl.ID);
            if (g != null)
                g.GetComponent<OneCardManager>().CanBePlayedNow = cl.CanBePlayed && !removeAllHighlights && !UIManager.Instance.IsDraggingCard && !UIManager.Instance.IsSelectingTile;
        }

        foreach (CreatureLogic crl in ChessboardManager.Instance.chessboard.creaturesOnTile)
        {
            if (crl != null)
            {
                GameObject g = IDHolder.GetGameObjectWithID(crl.uniqueCreatureID);
                if(g != null)
                    g.GetComponent<OneCreatureManager>().CanAttackNow = (crl.AttacksLeftThisTurn > 0) && crl.CanAttack && !removeAllHighlights;
            }
        }

        /*
        // highlight hero power
        playerArea.HeroPower.Highlighted = (!usedHeroPowerThisTurn) && (ManaLeft > 1) && !removeAllHighlights;
        */
    }

    // START GAME METHODS
    public void LoadCharacterInfoFromAsset()
    {
        if (playerTeam == PlayerTeam.Red)
        {
            Vector2Int redKingTileIndex = new Vector2Int(1,0);
            CreatureLogic creatureKing = new CreatureLogic(this, charAsset, redKingTileIndex);
            ChessboardManager.Instance.chessboard.creaturesOnTile[redKingTileIndex.x, redKingTileIndex.y] = creatureKing;
            new InitKingCommand(this, redKingTileIndex, creatureKing.uniqueCreatureID).AddToQueue();
        }
        else if (playerTeam == PlayerTeam.Blue)
        {
            Vector2Int blueKingTileIndex = new Vector2Int(1,2);
            CreatureLogic creatureKing = new CreatureLogic(this, charAsset, blueKingTileIndex);
            ChessboardManager.Instance.chessboard.creaturesOnTile[blueKingTileIndex.x, blueKingTileIndex.y] = creatureKing;
            new InitKingCommand(this, blueKingTileIndex, creatureKing.uniqueCreatureID).AddToQueue();
        }
        
        /*
        // TODO: insert the code to attach hero power script here. 
        if (charAsset.HeroPowerName != null && charAsset.HeroPowerName != "")
        {
            HeroPowerEffect = System.Activator.CreateInstance(System.Type.GetType(charAsset.HeroPowerName)) as SpellEffect;
        }
        else
        {
            Debug.LogWarning("Check hero powr name for character " + charAsset.ClassName);
        }
        */
    }

    public void TransmitInfoAboutPlayerToVisual()
    {
        /*
        playerArea.Portrait.GetComponent<IDHolder>().UniqueID = playerID;
        */
        if (GetComponent<TurnMaker>() is AITurnMaker)
        {
            // turn off turn making for this character
            playerArea.AllowedToControlThisPlayer = false;
        }
        else
        {
            // allow turn making for this character
            playerArea.AllowedToControlThisPlayer = true;
        }
    }

    public void UseHeroPower()
    {
        /*
        ManaLeft -= 2;
        usedHeroPowerThisTurn = true;
        HeroPowerEffect.ActivateEffect();
        */
    }
}