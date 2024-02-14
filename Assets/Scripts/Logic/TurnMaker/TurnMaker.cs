using UnityEngine;
using System.Collections;

public abstract class TurnMaker : MonoBehaviour {

    protected Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    public virtual void OnTurnStart()
    {
        // add one mana crystal to the pool;
        player.OnTurnStart();
    }

}
