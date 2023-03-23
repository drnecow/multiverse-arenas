using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Grapple", menuName = "Scriptable Objects/Common Action/Grapple")]
public class Grapple : CombatAction
{
    private void Awake()
    {
        _name = "Grapple";
        Identifier = CombatActionType.Grapple;
    }

    public override void DoAction(Monster actor, Monster target)
    {
        Debug.Log($"{actor.Name} is trying to Grapple {target.Name}");
    }
}
