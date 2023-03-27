using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Dash", menuName = "Scriptable Objects/Common Action/Dash")]
public class Dash : CombatAction
{
    private GridMap _map;


    private void Awake()
    {
        _name = "Dash";
        Identifier = CombatActionType.Dash;
    }

    public override void DoAction(Monster actor, List<Coords> path)
    {
        base.DoAction(actor);

        if (_map == null)
            _map = CombatDependencies.Instance.Map;

        string pathString = "";

        foreach (Coords pathCoord in path)
            pathString += $"({pathCoord.x}, {pathCoord.y}) ";

        Debug.Log($"{actor.Name} is Dashing along path {pathString}");
    }
}
