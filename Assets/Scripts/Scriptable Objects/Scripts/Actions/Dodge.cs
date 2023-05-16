using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Dodge", menuName = "Scriptable Objects/Common Action/Dodge")]
public class Dodge : CombatAction
{
    private void Awake()
    {
        Name = "Dodge";
        Identifier = MonsterActionType.Dodge;
    }

    public override void DoAction(Monster actor, CombatActionType consumedAction)
    {
        base.DoAction(actor, consumedAction);

        Debug.Log($"{actor.Name} is Dodging");

        actor.AddActiveCondition(Condition.Dodging);
        InvokeOnActionAnimationStartedPlaying(actor, ConstantValues.ZERO_TIME_ANIMATIONS_DURATION);
    }

    public override bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType usedAction)
    {
        return base.CheckPlayerButtonInteractabilityCondition(actor, usedAction);
    }
}

