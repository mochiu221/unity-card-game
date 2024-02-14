using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CreatureDraggingController : MonoBehaviour {

    // PRIVATE FIELDS

    // a flag to know if we are currently dragging this GameObject
    private bool dragging = false;

    // distance from the center of this Game Object to the point where we clicked to start dragging 
    private Vector3 pointerDisplacement;

    // distance from camera to mouse on Z axis 
    private float zDisplacement;

    // reference to DraggingActions script. Dragging Actions should be attached to the same GameObject.
    private CreatureDraggingActions da;

    // STATIC property that returns the instance of Draggable that is currently being dragged
    private static CreatureDraggingController draggingThis;
    public static CreatureDraggingController DraggingThis
    {
        get{ return draggingThis;}
    }

    Camera mainCamera;

    // MONOBEHAVIOUR METHODS
    void Awake()
    {
        mainCamera = Camera.main;
        da = GetComponent<CreatureDraggingActions>();
    }

    void OnMouseDown()
    {
        if (da!=null && da.CanDrag)
        {
            dragging = true;
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