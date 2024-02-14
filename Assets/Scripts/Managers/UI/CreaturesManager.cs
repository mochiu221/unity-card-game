using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager : MonoBehaviour
{
    public static CreaturesManager Instance { set; get; }

    [Header("Creature Frames")]
    public Sprite myLostFrame;
    public Sprite mySoldierFrame;
    public Sprite myStrikerFrame;
    public Sprite myArcherFrame;
    public Sprite myAssassinFrame;
    public Sprite myThroneFrame;
    public Sprite myKingFrame;
    public Sprite enemyLostFrame;
    public Sprite enemySoldierFrame;
    public Sprite enemyStrikerFrame;
    public Sprite enemyArcherFrame;
    public Sprite enemyAssassinFrame;
    public Sprite enemyThroneFrame;
    public Sprite enemyKingFrame;

    private void Awake() 
    {
        Instance = this;
    }
}