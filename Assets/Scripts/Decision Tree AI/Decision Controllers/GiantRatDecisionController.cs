using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public class GiantRatDecisionController : MonsterDecisionController
{
    private MeleeAttack _biteAttack;


    protected override void Awake()
    {
        base.Awake();
        _biteAttack = _actor.CombatActions.FindMeleeAttackOfType(AttackType.BiteAttack);
        _biteAttack.OnAttackAnimationStartedPlaying += SwitchToCurrentAnimationClip;
    }

    public override void BuildDecisionTree()
    {
        DTreeLeaf attackGrappler = new DTreeLeaf("Attack grappler",
            () => DoSequenceOfActionsAndEndTurn(
                () => _biteAttack.MakeMeleeAttack(_actor, _actor.ConditionSourceMonsters[Condition.Grappled])));
        DTreeLeaf tryEscapeGrapple = new DTreeLeaf("Try to escape grapple",
            () => _actor.EscapeGrapple(false));

        DTreeLeaf attackEnemyInMelee = new DTreeLeaf("Attack enemy in melee reach",
            () => DoSequenceOfActionsAndEndTurn(() => _biteAttack.MakeMeleeAttack(_actor, _closestTarget)));

        DTreeLeaf moveToClosestEnemyAndAttack = new DTreeLeaf("Move to closest enemy and attack it",
            () => DoSequenceOfActionsAndEndTurn(() => _commonActions[CombatActionType.Move].DoAction(_actor, _pathToTarget),
                                                () => _biteAttack.MakeMeleeAttack(_actor, _closestTarget)));
        DTreeLeaf dashToClosestEnemy = new DTreeLeaf("Dash to closest enemy",
            () => DoSequenceOfActionsAndEndTurn(() => _commonActions[CombatActionType.Dash].DoAction(_actor, _pathToTarget)));

        DTreeLeaf takeDodgeAction = new DTreeLeaf("Take Dodge action",
            () => DoSequenceOfActionsAndEndTurn(() => _actor.CombatActions.FindMainActionOfType(CombatActionType.Dodge).DoAction(_actor)));

        DTreeLeaf skipTurn = new DTreeLeaf("Nothing to do, skip turn", SkipTurn);


        DTreeRoot root = new DTreeRoot();


        DTreeBinaryConditional doIHaveMainAction = new DTreeBinaryConditional("Do I have main action?", () => _actor.MainActionAvailable);


        DTreeBinaryConditional amIGrappled = new DTreeBinaryConditional("Am I grappled?", () => _actor.ActiveConditions.Contains(Condition.Grappled));

        
        DTreeRandomChoice attackGrapplerOrTryEscape = new DTreeRandomChoice("Attack grappler or try to escape grapple (50/50)");

        attackGrapplerOrTryEscape.AddChild(attackGrappler);
        attackGrapplerOrTryEscape.AddChild(tryEscapeGrapple);


        DTreeBinaryConditional doISeeEnemies = new DTreeBinaryConditional("Do I see enemies?", () => _actor.VisibleTargets.Count > 0);


        DTreeBinaryConditional isEnemyInMyMeleeReach = new DTreeBinaryConditional("Is an enemy in my melee reach?", 
            () => { List<Monster> enemiesInMeleeReach = FindEnemiesInMeleeAttackReach(_biteAttack);
                if (enemiesInMeleeReach.Count > 0)
                    _closestTarget = enemiesInMeleeReach[0];
                return enemiesInMeleeReach.Count > 0; 
            });
        

        DTreeBinaryConditional doIHaveEnoughSpeedToReachClosestEnemyThisTurn = new DTreeBinaryConditional("Do I have enough speed to reach closest enemy on this turn?", 
            () => { _closestTarget = FindClosestEnemy(out _pathToTarget);
                if (_closestTarget != null)
                    return _actor.Stats.Speed.GetSpeedCells(Speed.Walk) >= _pathToTarget.Count;
                else
                    return false;
            });

        
        doIHaveEnoughSpeedToReachClosestEnemyThisTurn.SetTrueConditionChild(moveToClosestEnemyAndAttack);
        doIHaveEnoughSpeedToReachClosestEnemyThisTurn.SetFalseConditionChild(dashToClosestEnemy);

        isEnemyInMyMeleeReach.SetTrueConditionChild(attackEnemyInMelee);
        isEnemyInMyMeleeReach.SetFalseConditionChild(doIHaveEnoughSpeedToReachClosestEnemyThisTurn);

        doISeeEnemies.SetTrueConditionChild(isEnemyInMyMeleeReach);
        doISeeEnemies.SetFalseConditionChild(takeDodgeAction);

        amIGrappled.SetTrueConditionChild(attackGrapplerOrTryEscape);
        amIGrappled.SetFalseConditionChild(doISeeEnemies);

        doIHaveMainAction.SetTrueConditionChild(amIGrappled);
        doIHaveMainAction.SetFalseConditionChild(skipTurn);

        root.SetChild(doIHaveMainAction);
        _decisionTreeRoot = root;
    }
}
