using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Disengage", menuName = "Scriptable Objects/Common Action/Disengage")]
public class Disengage : CombatAction
{
    private void Awake()
    {
        _name = "Disengage";
        Identifier = CombatActionType.Disengage;
    }

    public override void DoAction(Monster actor)
    {
        base.DoAction(actor);

        Debug.Log($"{actor.Name} is Disengaging");
    }
}
