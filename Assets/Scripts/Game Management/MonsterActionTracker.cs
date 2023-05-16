using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterActionTracker : MonoBehaviour
{
    private HashSet<CombatAction> _gameActions;
    private HashSet<Attack> _gameAttacks;


    private void Awake()
    {
        _gameActions = new HashSet<CombatAction>();
        _gameAttacks = new HashSet<Attack>();
    }
    public void AddCombatAction(CombatAction combatAction)
    {
        if (!_gameActions.Contains(combatAction))
        {
            _gameActions.Add(combatAction);
            combatAction.OnActionAnimationStartedPlaying += (actionArguments) => StartCoroutine(WaitForActionToFinish(actionArguments));
        }
    }
    public void AddAttack(Attack attack)
    {
        if (!_gameAttacks.Contains(attack))
        {
            _gameAttacks.Add(attack);
            attack.OnAttackAnimationStartedPlaying += (attackArguments) => StartCoroutine(WaitForActionToFinish(attackArguments));
        }
    }

    private IEnumerator WaitForActionToFinish(ActionAnimationStartArguments actionArguments)
    {
        yield return new WaitForSeconds(actionArguments.AnimationDurationInSeconds);
        actionArguments.InvokeAnimationEndEvent();
    }
}
