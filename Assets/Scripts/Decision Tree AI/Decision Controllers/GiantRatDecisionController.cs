using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            () => DoSequenceOfActionsAndEndTurn(() => _actor.EscapeGrapple(false)));

        DTreeLeaf attackEnemyInMelee = new DTreeLeaf("Attack enemy in melee reach",
            () => DoSequenceOfActionsAndEndTurn(() => _biteAttack.MakeMeleeAttack(_actor, _closestTarget)));

        DTreeLeaf moveToClosestEnemyAndAttack = new DTreeLeaf("Move to closest enemy and attack it",
            () => { Action moveAction = () => _commonActions[MonsterActionType.Move].DoAction(_actor, _pathToTarget, CombatActionType.FreeAction);
                DoSequenceOfActionsAndEndTurn(moveAction, () => _biteAttack.MakeMeleeAttack(_actor, _closestTarget));
            });

        DTreeLeaf moveToClosestObstacleAndHide = new DTreeLeaf("Move to closest obstacle and hide",
            () =>
            {
                Action moveAction = () => _commonActions[MonsterActionType.Move].DoAction(_actor, _pathToObstacle, CombatActionType.FreeAction);
                DoSequenceOfActionsAndEndTurn(moveAction, () => _commonActions[MonsterActionType.Hide].DoAction(_actor, CombatActionType.MainAction));
            });

        DTreeLeaf moveAndDashToClosestEnemy = new DTreeLeaf("Move and dash to closest enemy",
            () => {
                Action dashAction;
                Action moveAction;

                int remainingSpeedCells = _actor.RemainingSpeed.GetSpeedCells(Speed.Walk);

                moveAction = () =>
                    _commonActions[MonsterActionType.Move].DoAction(_actor, _pathToTarget.GetRange(0, remainingSpeedCells), CombatActionType.FreeAction);
                dashAction = () =>
                    _commonActions[MonsterActionType.Dash].
                        DoAction(_actor,
                                 _pathToTarget.GetRange(remainingSpeedCells,
                                    Mathf.Min(_actor.Stats.Speed.GetSpeedCells(Speed.Walk), _pathToTarget.Count - remainingSpeedCells)),
                                 CombatActionType.MainAction);

                DoSequenceOfActionsAndEndTurn(moveAction, dashAction);
            });
        DTreeLeaf moveToClosestEnemy = new DTreeLeaf("Move to closest enemy", () =>
        {
            Action moveAction = () => _commonActions[MonsterActionType.Move].
                DoAction(_actor, _pathToTarget.GetRange(0, _actor.RemainingSpeed.GetSpeedCells(Speed.Walk)), CombatActionType.FreeAction);

            DoSequenceOfActionsAndEndTurn(moveAction);
        });

        DTreeLeaf takeSeekAction = new DTreeLeaf("Take Seek action",
            () => DoSequenceOfActionsAndEndTurn(() => _commonActions[MonsterActionType.Seek].DoAction(_actor, CombatActionType.MainAction)));

        DTreeLeaf takeDodgeAction = new DTreeLeaf("Take Dodge action",
            () => DoSequenceOfActionsAndEndTurn(() => _commonActions[MonsterActionType.Dodge].DoAction(_actor, CombatActionType.MainAction)));

        DTreeLeaf skipTurn = new DTreeLeaf("Nothing to do, skip turn", SkipTurn);


        DTreeRoot root = new DTreeRoot();


        DTreeBinaryConditional doIHaveMainAction = new DTreeBinaryConditional("Do I have main action?", () => _actor.MainActionAvailable);


        DTreeBinaryConditional amIGrappled = new DTreeBinaryConditional("Am I grappled?", () => _actor.ActiveConditions.Contains(Condition.Grappled));

        
        DTreeRandomChoice attackGrapplerOrTryEscape = new DTreeRandomChoice("Attack grappler or try to escape grapple (50/50)");

        attackGrapplerOrTryEscape.AddChild(attackGrappler);
        attackGrapplerOrTryEscape.AddChild(tryEscapeGrapple);


        DTreeBinaryConditional doISeeEnemies = new DTreeBinaryConditional("Do I see enemies?", () => _actor.VisibleTargets.Count > 0);


        DTreeBinaryConditional isEnemyInMyMeleeReach = new DTreeBinaryConditional("Is an enemy in my melee reach?", 
            () => { HashSet<Monster> enemiesInMeleeReach = FindEnemiesInMeleeAttackReach(_biteAttack);
                if (enemiesInMeleeReach.Count > 0)
                    _closestTarget = enemiesInMeleeReach.ToList()[0];
                return enemiesInMeleeReach.Count > 0; 
            });

        DTreeBinaryConditional isThereAPathToEnemyAtAll = new DTreeBinaryConditional("Is there a path to any enemy at all?", 
            () =>
            {
                _closestTarget = FindClosestEnemy(out _pathToTarget);

                if (_closestTarget == null)
                    return false;
                else
                    return true;
            });

        DTreeBinaryConditional doIHaveEnoughSpeedToReachClosestEnemyThisTurn = new DTreeBinaryConditional("Do I have enough speed to reach closest enemy on this turn?",
            () => _actor.Stats.Speed.GetSpeedCells(Speed.Walk) >= _pathToTarget.Count);

        DTreeBinaryConditional doIHaveEnoughSpeedToReachClosestObstacleThisTurn = new DTreeBinaryConditional("Do I have enough speed to reach closest obstacle on this turn?",
            () =>
            {
                _pathToObstacle = FindPathToClosestObstacle();

                if (_pathToObstacle != null)
                    return _actor.Stats.Speed.GetSpeedCells(Speed.Walk) >= _pathToObstacle.Count;
                else
                    return false;
            });

        DTreeRandomChoice hideOrMoveAndDashToClosestEnemy = new DTreeRandomChoice("Hide or move and dash to closest enemy (50/50)");

        DTreeBinaryConditional amIHidden = new DTreeBinaryConditional("Am I hidden?", () => _actor.ActiveConditions.Contains(Condition.Hiding));

        //amIHidden.SetTrueConditionChild(moveToClosestEnemy);
        //amIHidden.SetFalseConditionChild(moveToClosestObstacleAndHide);

        //hideOrMoveAndDashToClosestEnemy.AddChild(amIHidden);
        //hideOrMoveAndDashToClosestEnemy.AddChild(moveAndDashToClosestEnemy);

        //doIHaveEnoughSpeedToReachClosestObstacleThisTurn.SetTrueConditionChild(hideOrMoveAndDashToClosestEnemy);
        //doIHaveEnoughSpeedToReachClosestObstacleThisTurn.SetFalseConditionChild(moveAndDashToClosestEnemy);
        
        doIHaveEnoughSpeedToReachClosestEnemyThisTurn.SetTrueConditionChild(moveToClosestEnemyAndAttack);
        doIHaveEnoughSpeedToReachClosestEnemyThisTurn.SetFalseConditionChild(moveAndDashToClosestEnemy);

        isThereAPathToEnemyAtAll.SetTrueConditionChild(doIHaveEnoughSpeedToReachClosestEnemyThisTurn);
        isThereAPathToEnemyAtAll.SetFalseConditionChild(takeDodgeAction);

        isEnemyInMyMeleeReach.SetTrueConditionChild(attackEnemyInMelee);
        isEnemyInMyMeleeReach.SetFalseConditionChild(isThereAPathToEnemyAtAll);

        doISeeEnemies.SetTrueConditionChild(isEnemyInMyMeleeReach);
        doISeeEnemies.SetFalseConditionChild(takeSeekAction);

        amIGrappled.SetTrueConditionChild(attackGrapplerOrTryEscape);
        amIGrappled.SetFalseConditionChild(doISeeEnemies);

        doIHaveMainAction.SetTrueConditionChild(amIGrappled);
        doIHaveMainAction.SetFalseConditionChild(skipTurn);

        root.SetChild(doIHaveMainAction);
        _decisionTreeRoot = root;
    }
}
