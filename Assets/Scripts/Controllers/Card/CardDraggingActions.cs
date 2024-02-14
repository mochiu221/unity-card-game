using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CardDraggingActions : MonoBehaviour {

    public abstract void OnStartDrag();

    public abstract void OnEndDrag();

    public abstract void OnDraggingInUpdate();

    public abstract void BackToHand();

    public abstract void SelectingTargetState();

    public abstract void PlayCardOnTarget();

    public OneTileManager tileManagerOnHover;

    public int numberOfTarget = 1;
    public List<OneTileManager> tilesList;

    public abstract CardLogic GetCardLogic();

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
            
            PlayerTeam pt =  GetComponent<OneCardManager>().playerTeam;
            if (pt == PlayerTeam.Red)
                return PlayersManager.Instance.PlayersByTeam[PlayerTeam.Red];
            else if (pt == PlayerTeam.Blue)
                return PlayersManager.Instance.PlayersByTeam[PlayerTeam.Blue];
            else
            {
                return null;
            }
        }
    }

    protected abstract bool DragSuccessful();
}
