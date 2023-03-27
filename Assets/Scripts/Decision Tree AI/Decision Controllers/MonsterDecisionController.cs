using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;

public class MonsterDecisionController : MonoBehaviour
{
    protected Monster _actor;
    protected Monster _closestTarget;
    protected List<Coords> _pathToTarget;

    protected GridMap _map;
    protected DTreeRoot _decisionTreeRoot;

    protected Dictionary<CombatActionType, CombatAction> _commonActions;

    public event Action OnEndTurn;
    protected bool _currentAnimationStartedPlaying = false;
    protected float _currentAnimationLength;


    protected virtual void Awake()
    {
        _actor = gameObject.GetComponent<Monster>();

        _commonActions = new Dictionary<CombatActionType, CombatAction>()
        {
            {  CombatActionType.Move, _actor.CombatActions.FindFreeActionOfType(CombatActionType.Move) },
            {  CombatActionType.Dash, _actor.CombatActions.FindMainActionOfType(CombatActionType.Dash) },
            {  CombatActionType.Disengage, _actor.CombatActions.FindMainActionOfType(CombatActionType.Disengage) },
            {  CombatActionType.Dodge, _actor.CombatActions.FindMainActionOfType(CombatActionType.Dodge) },
            {  CombatActionType.Grapple, _actor.CombatActions.FindMainActionOfType(CombatActionType.Grapple) },
            {  CombatActionType.Hide, _actor.CombatActions.FindMainActionOfType(CombatActionType.Hide) },
            {  CombatActionType.Seek, _actor.CombatActions.FindMainActionOfType(CombatActionType.Seek) },
        };

        foreach (CombatAction commonAction in _commonActions.Values)
            commonAction.OnActionAnimationStartedPlaying += SwitchToCurrentAnimationClip;

        OnEndTurn += () => Debug.Log($"{_actor} ended turn");
    }


    public virtual void BuildDecisionTree()
    {

    }
    [ContextMenu("Make Decision")]
    public void MakeDecision()
    {
        if (_decisionTreeRoot == null)
            BuildDecisionTree();

        if (_map == null)
            _map = _actor.CombatDependencies.Map;

        Action decision = _decisionTreeRoot.RunNodeProcess();

        if (decision != null)
            decision();
        else
            Debug.LogWarning("Cannot make decision, something went wrong during tree traversal");
    }
    [ContextMenu("Print Decision Tree")]
    public void PrintDecisionTree()
    {
        if (_decisionTreeRoot == null)
            BuildDecisionTree();

        _decisionTreeRoot.PrintTree();
    }

    public List<Monster> FindEnemiesInMeleeAttackReach(MeleeAttack attack)
    {
        List<Monster> enemies = new List<Monster>();
        List<Monster> surroundingMonsters = _map.FindMonstersInRadius(_actor.CurrentCoordsOriginCell, _actor.Stats.Size, attack.Reach);

        foreach (Monster monster in surroundingMonsters)
        {
            Debug.Log(monster);
            if (monster.IsPlayerControlled && _actor.VisibleTargets.Contains(monster))
                enemies.Add(monster);
        }

        return enemies;
    }
    public Monster FindClosestEnemy(out List<Coords> pathToClosestEnemy)
    {
        Dictionary<Monster, List<Coords>> pathsToAvailableEnemies = new Dictionary<Monster, List<Coords>>();
        Coords actorOriginCell = _actor.CurrentCoordsOriginCell;

        foreach (Monster enemy in _actor.VisibleTargets)
        {
            List<Coords> pathToEnemy = _map.FindPathForSingleCellEntity(actorOriginCell, enemy.CurrentCoordsOriginCell, true);

            if (pathToEnemy != null)
                pathsToAvailableEnemies.Add(enemy, pathToEnemy);
        }

        if (pathsToAvailableEnemies.Count == 0)
        {
            pathToClosestEnemy = null;
            return null;
        }

        Monster closestEnemy = pathsToAvailableEnemies.First().Key;
        pathToClosestEnemy = pathsToAvailableEnemies.First().Value;

        foreach (Monster availableEnemy in pathsToAvailableEnemies.Keys)
        {
            List<Coords> currentPath = pathsToAvailableEnemies[availableEnemy];

            if (currentPath.Count < pathToClosestEnemy.Count)
            {
                closestEnemy = availableEnemy;
                pathToClosestEnemy = currentPath;
            }
        }

        return closestEnemy;
    }
    public void DoSequenceOfActionsAndEndTurn(Action monsterAction, params Action[] additionalActions)
    {
        List<Action> monsterActions = new List<Action>() { monsterAction };

        foreach (Action additionalAction in additionalActions)
            monsterActions.Add(additionalAction);

        StartCoroutine(WaitForActionAnimationsToFinish(monsterActions));
    }
    private IEnumerator WaitForActionAnimationsToFinish(List<Action> monsterActions)
    {
        foreach (Action monsterAction in monsterActions)
        {
            _currentAnimationStartedPlaying = false;
            monsterAction();

            while (!_currentAnimationStartedPlaying)
                yield return null;

            yield return new WaitForSeconds(_currentAnimationLength + ConstantValues.ANIMATIONS_SWITCH_SPEED);
        }

        Debug.Log($"Total actions played: {monsterActions.Count}");
        OnEndTurn?.Invoke();
    }
    protected void SwitchToCurrentAnimationClip(float currentClipLength)
    {
        _currentAnimationStartedPlaying = true;
        _currentAnimationLength = currentClipLength;
    }
    public void SkipTurn()
    {
        OnEndTurn?.Invoke();
    }
}
