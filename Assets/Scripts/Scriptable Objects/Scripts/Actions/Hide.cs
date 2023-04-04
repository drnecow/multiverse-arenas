using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Hide", menuName = "Scriptable Objects/Common Action/Hide")]
public class Hide : CombatAction
{
    private void Awake()
    {
        Name = "Hide";
        Identifier = MonsterActionType.Hide;
    }

    public override void DoAction(Monster actor, CombatActionType consumedAction)
    {
        base.DoAction(actor, consumedAction);
        Debug.Log($"{actor.Name} is Hiding");

        int stealthRoll = actor.MakeSkillCheck(Skill.Stealth);
        actor.StealthRoll = stealthRoll;
        actor.CombatDependencies.CombatManager.HandleMonsterEnteringStealth(actor, stealthRoll);
    }

    public override bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType usedAction)
    {
        GridMap map = actor.CombatDependencies.Map;

        List<MultipleCellLine> neighbours = map.GetNeighboursForMultipleCellEntity(actor.CurrentCoordsOriginCell, GridMap.GetSquareSideForEntitySize(actor.Stats.Size));
        bool areThereAdjacentObstacles = false;

        foreach (MultipleCellLine neighbourLine in neighbours)
            if (neighbourLine.Coords.Any(coord => map.GetGridObjectAtCoords(coord).HasImpassableObstacle))
                areThereAdjacentObstacles = true;

        bool isUsedActionConsumed = base.CheckPlayerButtonInteractabilityCondition(actor, usedAction);

        return areThereAdjacentObstacles && isUsedActionConsumed;
    }
}
