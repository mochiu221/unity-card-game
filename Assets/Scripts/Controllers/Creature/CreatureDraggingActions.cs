using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CreatureDraggingActions : MonoBehaviour {

    public abstract void OnStartDrag();

    public abstract void OnEndDrag();

    public abstract void OnDraggingInUpdate();

    public virtual bool CanDrag
    {
        get
        {            
            return PlayersManager.Instance.CanControlThisPlayer(playerOwner) && !UIManager.Instance.IsSelectingTile;
        }
    }

    protected virtual Player playerOwner
    {
        get{
            
            PlayerTeam pt =  GetComponentInParent<OneCreatureManager>().playerTeam;
            if (pt == PlayerTeam.Red)
                return PlayersManager.Instance.PlayersByTeam[PlayerTeam.Red];
            else if (pt == PlayerTeam.Blue)
                return PlayersManager.Instance.PlayersByTeam[PlayerTeam.Blue];
            else
            {
                Debug.LogError("Untagged Card or creature " + transform.parent.name);
                return null;
            }
        }
    }

    protected abstract bool DragSuccessful();
}