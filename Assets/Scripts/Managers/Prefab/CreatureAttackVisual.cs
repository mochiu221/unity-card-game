using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CreatureAttackVisual : MonoBehaviour 
{
    private OneCreatureManager manager;
    private CreatureVisualController cvc;

    void Awake()
    {
        manager = GetComponent<OneCreatureManager>();    
        cvc = GetComponent<CreatureVisualController>();
    }

    public void AttackTarget(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int attackerHealthAfter, int targetHealthAfter)
    {
        manager.CanAttackNow = false;
        GameObject target = IDHolder.GetGameObjectWithID(targetUniqueID);

        // bring this creature to front sorting-wise.
        cvc.BringToFront();
        CreatureVisualStates tempState = cvc.VisualState;
        cvc.VisualState = CreatureVisualStates.Transition;

        transform.DOMove(target.transform.position, 0.4f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InCubic).OnComplete(() =>
            {
                // if(damageTakenByTarget>0)
                //     DamageEffect.CreateDamageEffect(target.transform.position, damageTakenByTarget);
                // if(damageTakenByAttacker>0)
                //     DamageEffect.CreateDamageEffect(transform.position, damageTakenByAttacker);
                
                target.GetComponent<OneCreatureManager>().healthText.text = targetHealthAfter.ToString();

                cvc.SetTableSortingOrder();
                cvc.VisualState = tempState;

                manager.healthText.text = attackerHealthAfter.ToString();
                Sequence s = DOTween.Sequence();
                s.AppendInterval(0.5f);
                s.OnComplete(Command.CommandExecutionComplete);
                //Command.CommandExecutionComplete();
            });
    }
        
}
