using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// an enum to store the info about where this object is
public enum CreatureVisualStates
{
    Transition,
    Dragging
}

public class CreatureVisualController : MonoBehaviour {

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

    private CreatureVisualStates state;
    public CreatureVisualStates VisualState
    {
        get{ return state; }  

        set
        {
            state = value;
            switch (state)
            {
                case CreatureVisualStates.Transition:
                    // hover.ThisPreviewEnabled = false;
                    break;
                case CreatureVisualStates.Dragging:
                    // hover.ThisPreviewEnabled = false;
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

    public void SetTableSortingOrder()
    {
        canvas.sortingOrder = 0;
        canvas.sortingLayerName = "Creatures";
    }


}