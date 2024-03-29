using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// This class enables Drag and Drop Behaviour for the game object it is attached to. 
/// It uses other script - DraggingActions to determine whether we can drag this game object now or not and 
/// whether the drop was successful or not.
/// </summary>

public class CardDraggingController : MonoBehaviour {

    // PRIVATE FIELDS

    // a flag to know if we are currently dragging this GameObject
    private bool dragging = false;

    // distance from the center of this Game Object to the point where we clicked to start dragging 
    private Vector3 pointerDisplacement;

    // distance from camera to mouse on Z axis 
    private float zDisplacement;

    // reference to DraggingActions script. Dragging Actions should be attached to the same GameObject.
    private CardDraggingActions da;

    // STATIC property that returns the instance of Draggable that is currently being dragged
    private static CardDraggingController draggingThis;
    public static CardDraggingController DraggingThis
    {
        get{ return draggingThis;}
    }

    Camera mainCamera;

    // MONOBEHAVIOUR METHODS
    void Awake()
    {
        mainCamera = Camera.main;
        da = GetComponent<CardDraggingActions>();
    }

    void OnMouseDown()
    {
        if (da!=null && da.CanDrag)
        {
            dragging = true;
            UIManager.Instance.IsDraggingCard = true;
            // when we are dragging something, all previews should be off
            UIManager.PreviewsAllowed = false;
            draggingThis = this;
            da.OnStartDrag();
            zDisplacement = -Camera.main.transform.position.z + transform.position.z;
            pointerDisplacement = -transform.position + MouseInWorldCoords();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (dragging)
        { 
            Vector3 mousePos = MouseInWorldCoords();
            //Debug.Log(mousePos);
            transform.position = new Vector3(mousePos.x - pointerDisplacement.x, mousePos.y - pointerDisplacement.y, transform.position.z);   
            da.OnDraggingInUpdate();
        }
    }
	
    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            UIManager.Instance.IsDraggingCard = false;
            // turn all previews back on
            UIManager.PreviewsAllowed = true;
            draggingThis = null;
            da.OnEndDrag();
        }
    }   

    // returns mouse position in World coordinates for our GameObject to follow. 
    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        //Debug.Log(screenMousePos);
        screenMousePos.z = zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
        
}
