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

    public override void DoAction(Monster actor)
    {
        base.DoAction(actor);

        Debug.Log($"{actor.Name} is Dodging");

        actor.IsDodging = true;
        OnActionAnimationStartedPlayingInvoke(actor, 0.5f);
    }
}

