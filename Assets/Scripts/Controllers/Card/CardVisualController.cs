using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// an enum to store the info about where this object is
public enum CardVisualStates
{
    Transition,
    Hand,
    Chessboard,
    Dragging
}

public class CardVisualController : MonoBehaviour {

    // reference to a canvas on this object to set sorting order
    private Canvas canvas;

    // a value for canvas sorting order when we want to show this object above everything
    private int TopSortingOrder = 500;

    // PROPERTIES
    private int slot = -1;
    public int Slot
    {
        get{ return slot;}

        set
        {
            slot = value;
            /*if (value != -1)
            {
                canvas.sortingOrder = HandSortingOrder(slot);
            }*/
        }
    }

    private CardVisualStates state;
    public CardVisualStates VisualState
    {
        get{ return state; }  

        set
        {
            state = value;
            switch (state)
            {
                case CardVisualStates.Hand:
                    // UIManager.PreviewsAllowed = true;
                    break;
                case CardVisualStates.Chessboard:
                    // UIManager.PreviewsAllowed = true;
                    break;
                case CardVisualStates.Transition:
                    // UIManager.PreviewsAllowed = false;
                    break;
                case CardVisualStates.Dragging:
                    // UIManager.PreviewsAllowed = false;
                    break;
            }
        }
    }

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    public void BringToFront()
    {
        canvas.sortingOrder = TopSortingOrder;
        canvas.sortingLayerName = "AboveEverything";
    }

    // not setting sorting order inside of VisualStaes property because when the card is drawn, 
    // we want to set an index first and set the sorting order only when the card arrives to hand. 
    public void SetHandSortingOrder()
    {
        if (slot != -1)
            canvas.sortingOrder = HandSortingOrder(slot);
        canvas.sortingLayerName = "Cards";
    }

    private int HandSortingOrder(int placeInHand)
    {
        return (-(placeInHand + 1) * 10); 
    }


}
