using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Disengage", menuName = "Scriptable Objects/Common Action/Disengage")]
public class Disengage : CombatAction
{
    // TODO: add removal of Disengage at the end of turn for player-controlled monsters

    private void Awake()
    {
        Name = "Disengage";
        Identifier = MonsterActionType.Disengage;
    }

    public override void DoAction(Monster actor, CombatActionType consumedAction)
    {
        base.DoAction(actor, consumedAction);

        Debug.Log($"{actor.Name} is Disengaging");

        actor.AddActiveCondition(Condition.Disengaging);
        OnActionAnimationStartedPlayingInvoke(actor, 0.5f);
    }

    public override bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType usedAction)
    {
        return false; // While opportunity attacks aren't implemented, Disengage is disabled
        //return base.CheckPlayerButtonInteractabilityCondition(actor, usedAction);
    }
}
