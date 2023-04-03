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

    public override void DoAction(Monster actor)
    {
        base.DoAction(actor);

        Debug.Log($"{actor.Name} is Disengaging");

        actor.IsDisengaging = true;
    }
}
