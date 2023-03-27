using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Move", menuName = "Scriptable Objects/Common Action/Move")]
public class Move : CombatAction
{
    private GridMap _map;


    private void Awake()
    {
        _name = "Move";
        Identifier = CombatActionType.Move;
    }

    public override void DoAction(Monster actor, List<Coords> path)
    {
        base.DoAction(actor);

        if (_map == null)
            _map = CombatDependencies.Instance.Map;

        actor.Animator.SetTrigger("Move");

        string pathString = "";

        foreach (Coords pathCoord in path)
            pathString += $"({pathCoord.x}, {pathCoord.y}) ";

        Debug.Log($"{actor.Name} is Moving along path {pathString}");
    }
}
