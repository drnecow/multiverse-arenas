using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Grapple", menuName = "Scriptable Objects/Common Action/Grapple")]
public class Grapple : CombatAction
{
    private void Awake()
    {
        Name = "Grapple";
        Identifier = MonsterActionType.Grapple;
    }

    public override void DoAction(Monster actor, Monster target, CombatActionType consumedAction)
    {
        base.DoAction(actor, consumedAction);

        Debug.Log($"{actor.Name} is trying to Grapple {target.Name}");
    }

    // TODO: implement this method properly
    public override bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType usedAction)
    {
        return base.CheckPlayerButtonInteractabilityCondition(actor, usedAction);
    }
}
