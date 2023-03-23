using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Dash", menuName = "Scriptable Objects/Common Action/Dash")]
public class Dash : CombatAction
{
    private void Awake()
    {
        _name = "Dash";
        Identifier = CombatActionType.Dash;
    }

    public override void DoAction(Monster actor, List<Coords> path)
    {
        string pathString = "";

        foreach (Coords pathCoord in path)
            pathString += $"({pathCoord.x}, {pathCoord.y}) ";

        Debug.Log($"{actor.Name} is Dashing along path {pathString}");
    }
}
