using UnityEngine;
using System.Collections;

public enum PlayerTeam
{
    Red = 0, 
    Blue = 1
}

// public enum PlayerPos
// {
//     MyPos = 0,
//     OpponentPos = 1
// }

public class PlayerArea : MonoBehaviour 
{
    public bool controlsON = true;
    public DeckVisual deckVisual;
    public ManaManager manaManager;
    public HandManager handManager;
    // public EndTurnButton EndTurnButton;
    // public ChessboardManager chessboardManager;

    public bool AllowedToControlThisPlayer
    {
        get;
        set;
    }      
}