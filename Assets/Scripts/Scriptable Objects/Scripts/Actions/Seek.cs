using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Seek", menuName = "Scriptable Objects/Common Action/Seek")]
public class Seek : CombatAction
{
    private void Awake()
    {
        Name = "Seek";
        Identifier = MonsterActionType.Seek;
    }

    public override void DoAction(Monster actor, CombatActionType consumedAction)
    {
        base.DoAction(actor, consumedAction);      

        Debug.Log($"{actor.Name} is Seeking");
        InvokeOnActionAnimationStartedPlaying(actor, ConstantValues.ZERO_TIME_ANIMATIONS_DURATION);

        Debug.Log(actor);
        Debug.Log(_combatDependencies.CombatManager);
        HashSet<Monster> enemies = actor.IsPlayerControlled ? _combatDependencies.CombatManager.EnemyMonsters : _combatDependencies.CombatManager.AlliedMonsters;
        List<Monster> hiddenEnemies = enemies.Where(enemy => enemy.ActiveConditions.Contains(Condition.Hiding)).ToList();

        Debug.Log($"Total enemies: {enemies.Count}");
        Debug.Log($"Hidden enemies: {hiddenEnemies.Count}");

        int perceptionCheck = actor.MakeSkillCheck(Skill.Perception);

        foreach (Monster enemy in hiddenEnemies)
            if (perceptionCheck > enemy.StealthRoll)
            {
                actor.VisibleTargets.Add(enemy);

                enemy.StealthRoll = -1000;
                enemy.RemoveActiveCondition(Condition.Hiding);
                enemy.MonsterAnimator.SetMonsterNormalMaterial();
                _combatDependencies.CombatManager.HandleMonsterBreakingStealth(actor);
                _combatDependencies.EventsLogger.LogLocalInfo(enemy, "Found", LogColor.Miss);
            }
    }

    public override bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType usedAction)
    {
        return base.CheckPlayerButtonInteractabilityCondition(actor, usedAction);
    }
}
