using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    // PUBLIC FIELDS
    public Player player;
    public bool takeCardsOpenly = true;
    public SameDistanceChildren slots;



    [Header("Transform References")]
    public Transform drawPreviewSpot;
    public Transform deckTransform;
    public Transform otherCardDrawSourceTransform;
    public Transform playPreviewSpot;
    public Transform selectEffectTargetSpot;


    // PRIVATE : a list of all card visual representations as GameObjects
    private List<GameObject> cardsInHand = new List<GameObject>();

    private void Start() 
    {
        takeCardsOpenly = player == PlayersManager.Instance.myPlayer;

        // Testing //
        // takeCardsOpenly = true;
    }

    // ADDING OR REMOVING CARDS FROM HAND

    // add a new card GameObject to hand
    public void AddCard(GameObject card)
    {
        // we allways insert a new card as 0th element in cardsInHand List 
        cardsInHand.Insert(0, card);

        // parent this card to our Slots GameObject
        card.transform.SetParent(slots.transform);
        card.transform.localScale = Vector3.one;

        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    // remove a card GameObject from hand
    public void RemoveCard(GameObject card)
    {
        // remove a card from the list
        cardsInHand.Remove(card);

        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    // remove card with a given index from hand
    public void RemoveCardAtIndex(int index)
    {
        cardsInHand.RemoveAt(index);
        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    // get a card GameObject with a given index in hand
    public GameObject GetCardAtIndex(int index)
    {
        return cardsInHand[index];
    }
        
    // MANAGING CARDS AND SLOTS

    // move Slots GameObject according to the number of cards in hand
    void UpdatePlacementOfSlots()
    {
        float posX;
        if (cardsInHand.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[cardsInHand.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        // tween Slots GameObject to new position in 0.3 seconds
        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    // shift all cards to their new slots
    void PlaceCardsOnNewSlots()
    {
        foreach (GameObject g in cardsInHand)
        {
            // tween this card to a new Slot
            g.transform.DOLocalMoveX(slots.Children[cardsInHand.IndexOf(g)].transform.localPosition.x, 0.3f);

            // card z index
            g.transform.localPosition = new Vector3(g.transform.localPosition.x, g.transform.localPosition.y, cardsInHand.IndexOf(g) * 0.01f);

            // apply correct sorting order and HandSlot value for later 
            CardVisualController cvc = g.GetComponent<CardVisualController>();
            cvc.Slot = cardsInHand.IndexOf(g);
            cvc.SetHandSortingOrder();
        }
    }

    // CARD DRAW METHODS

    // creates a card and returns a new card as a GameObject
    GameObject CreateACardAtPosition(CardAsset cardAsset, Vector3 position, Vector3 eulerAngles)
    {
        // Instantiate a card depending on its type
        GameObject card;
        if (cardAsset.cardType == CardType.Creature)
        {
            // this card is a creature card
            card = GameObject.Instantiate(GameManager.Instance.creatureCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
        }
        else
        {
            // this is a spell: checking for targeted or non-targeted spell
            card = GameObject.Instantiate(GameManager.Instance.spellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
        }

        // apply the look of the card based on the info from CardAsset
        OneCardManager manager = card.GetComponent<OneCardManager>();
        manager.cardAsset = cardAsset;
        manager.playerTeam = player.playerTeam;
        manager.ReadCardFromAsset();

        return card;
    }

    // gives player a new card from a given position
    public void GivePlayerACard(CardAsset cardAsset, int uniqueID, bool fast = false, bool fromDeck = true)
    {
        GameObject card;
        if (fromDeck)
            card = CreateACardAtPosition(cardAsset, deckTransform.position, new Vector3(0f, -180f, 0f));
        else
            card = CreateACardAtPosition(cardAsset, otherCardDrawSourceTransform.position, new Vector3(0f, -180f, 0f));

        // // Set a tag to reflect where this card is
        // foreach (Transform t in card.GetComponentsInChildren<Transform>())
        //     t.tag = playerTeam.ToString()+"Card";
        // pass this card to HandVisual class
        AddCard(card);

        // Bring card to front while it travels from draw spot to hand
        CardVisualController cvc = card.GetComponent<CardVisualController>();
        cvc.BringToFront();
        cvc.Slot = 0;

        // pass a unique ID to this card.
        IDHolder id = card.AddComponent<IDHolder>();
        id.UniqueID = uniqueID;

        // move card to the hand;
        Sequence s = DOTween.Sequence();
        if (!fast)
        {
            // Debug.Log ("Not fast!!!");
            s.Append(card.transform.DOMove(drawPreviewSpot.position, GameManager.Instance.cardTransitionTime));
            if (takeCardsOpenly)
                s.Insert(0f, card.transform.DORotate(Vector3.zero, GameManager.Instance.cardTransitionTime)); 
            else 
                s.Insert(0f, card.transform.DORotate(new Vector3(0f, 180f, 0f), GameManager.Instance.cardTransitionTime)); 
            s.AppendInterval(GameManager.Instance.cardPreviewTime);
            // displace the card so that we can select it in the scene easier.
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GameManager.Instance.cardTransitionTime));
        }
        else
        {
            // displace the card so that we can select it in the scene easier.
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GameManager.Instance.cardTransitionTimeFast));
            if (takeCardsOpenly)    
                s.Insert(0f,card.transform.DORotate(Vector3.zero, GameManager.Instance.cardTransitionTimeFast)); 
        }

        s.OnComplete(()=>ChangeLastCardStatusToInHand(card, cvc));
    }

    public void DestroyACard(CardAsset cardAsset, int uniqueID, bool fromDeck = true)
    {
        GameObject card;
        if (fromDeck)
            card = CreateACardAtPosition(cardAsset, deckTransform.position, new Vector3(0f, -180f, 0f));
        else
            card = CreateACardAtPosition(cardAsset, otherCardDrawSourceTransform.position, new Vector3(0f, -180f, 0f));

        CardVisualController cvc = card.GetComponent<CardVisualController>();
        cvc.BringToFront();
        cvc.Slot = 0;

        IDHolder id = card.AddComponent<IDHolder>();
        id.UniqueID = uniqueID;

        Sequence s = DOTween.Sequence();
        s.Append(card.transform.DOMove(drawPreviewSpot.position, GameManager.Instance.cardTransitionTime));
        if (takeCardsOpenly)
            s.Insert(0f, card.transform.DORotate(Vector3.zero, GameManager.Instance.cardTransitionTime)); 
        else 
            s.Insert(0f, card.transform.DORotate(new Vector3(0f, 180f, 0f), GameManager.Instance.cardTransitionTime)); 
        s.AppendInterval(GameManager.Instance.cardPreviewTime);

        s.OnComplete(()=>{
            Destroy(card);
            Command.CommandExecutionComplete();
        });
    }

    // this method will be called when the card arrived to hand 
    void ChangeLastCardStatusToInHand(GameObject card, CardVisualController cvc)
    {
        // TODO: Fix, Check if my hand
        // Debug.Log("player team: "+playerTeam);
        // Debug.Log("can view?: "+PlayersManager.Instance.CanViewThisPlayerHand(playerTeam));
        cvc.VisualState = CardVisualStates.Hand;

        // set correct sorting order
        cvc.SetHandSortingOrder();
        // end command execution for DrawACArdCommand
        Command.CommandExecutionComplete();
    }

   
    // PLAYING SPELLS

    // 2 Overloaded method to show a spell played from hand
    public void PlayASpellFromHand(int CardID)
    {
        GameObject card = IDHolder.GetGameObjectWithID(CardID);
        PlayASpellFromHand(card);
    }

    public void PlayASpellFromHand(GameObject CardVisual)
    {
        Command.CommandExecutionComplete();
        CardVisual.GetComponent<CardVisualController>().VisualState = CardVisualStates.Transition;
        RemoveCard(CardVisual);

        CardVisual.transform.SetParent(null);

        Sequence s = DOTween.Sequence();
        s.Append(CardVisual.transform.DOMove(playPreviewSpot.position, 0.7f));
        s.Insert(0f, CardVisual.transform.DORotate(Vector3.zero, 1f));
        s.AppendInterval(0.5f);
        s.OnComplete(()=>
            {
                //Command.CommandExecutionComplete();
                Destroy(CardVisual);
            });
    }


}
