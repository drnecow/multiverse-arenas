using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Dodge", menuName = "Scriptable Objects/Common Action/Dodge")]
public class Dodge : CombatAction
{
    private void Awake()
    {
        _name = "Dodge";
        Identifier = CombatActionType.Dodge;
    }

    public override void DoAction(Monster actor)
    {
        base.DoAction(actor);

        Debug.Log($"{actor.Name} is Dodging");
    }
}

