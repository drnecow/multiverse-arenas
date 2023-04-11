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

        OnActionAnimationStartedPlayingInvoke(actor, 0.5f);

        int stealthRoll = actor.MakeSkillCheck(Skill.Stealth);
        HashSet<Monster> enemies = actor.IsPlayerControlled ? actor.CombatDependencies.CombatManager.EnemyMonsters : actor.CombatDependencies.CombatManager.AlliedMonsters;

        Debug.Log($"Stealth roll: {stealthRoll}");
        if (enemies.All(enemy => enemy.Stats.GetPassiveSkillValue(Skill.Perception) < stealthRoll))
        {
            actor.StealthRoll = stealthRoll;
            actor.AddActiveCondition(Condition.Hiding);
            actor.MonsterAnimator.SetMonsterStealthMaterial();
            actor.CombatDependencies.CombatManager.HandleMonsterEnteringStealth(actor);
        }
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

        return areThereAdjacentObstacles && !actor.ActiveConditions.Contains(Condition.Hiding) && isUsedActionConsumed;
    }
}
