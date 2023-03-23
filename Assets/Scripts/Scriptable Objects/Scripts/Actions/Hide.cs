using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Hide", menuName = "Scriptable Objects/Common Action/Hide")]
public class Hide : CombatAction
{
    private void Awake()
    {
        _name = "Hide";
        Identifier = CombatActionType.Hide;
    }

    public override void DoAction(Monster actor)
    {
        Debug.Log($"{actor.Name} is Hiding");
    }
}
