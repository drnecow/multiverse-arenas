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

    protected List<Coords> _pathToObstacle;

    protected GridMap _map;
    protected DTreeRoot _decisionTreeRoot;

    protected Dictionary<MonsterActionType, CombatAction> _commonActions;

    public event Action OnEnemyEndTurn;
    protected bool _currentAnimationStartedPlaying = false;
    protected float _currentAnimationLength;


    protected virtual void Awake()
    {
        _actor = gameObject.GetComponent<Monster>();

        _commonActions = new Dictionary<MonsterActionType, CombatAction>()
        {
            {  MonsterActionType.Move, _actor.CombatActions.FindFreeActionOfType(MonsterActionType.Move) },
            {  MonsterActionType.Dash, _actor.CombatActions.FindMainActionOfType(MonsterActionType.Dash) },
            {  MonsterActionType.Disengage, _actor.CombatActions.FindMainActionOfType(MonsterActionType.Disengage) },
            {  MonsterActionType.Dodge, _actor.CombatActions.FindMainActionOfType(MonsterActionType.Dodge) },
            //{  MonsterActionType.Grapple, _actor.CombatActions.FindMainActionOfType(MonsterActionType.Grapple) },
            {  MonsterActionType.Hide, _actor.CombatActions.FindMainActionOfType(MonsterActionType.Hide) },
            {  MonsterActionType.Seek, _actor.CombatActions.FindMainActionOfType(MonsterActionType.Seek) },
        };

        foreach (CombatAction commonAction in _commonActions.Values)
            commonAction.OnActionAnimationStartedPlaying += (actionArguments) => SwitchToCurrentAnimationClip(actionArguments.Actor, actionArguments.AnimationDurationInSeconds);

        OnEnemyEndTurn += () => Debug.Log($"{_actor} ended turn");
        OnEnemyEndTurn += () => _actor.RemoveActiveCondition(Condition.Disengaging);
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

    public HashSet<Monster> FindEnemiesInMeleeAttackReach(MeleeAttack attack)
    {
        HashSet<Monster> enemies = new HashSet<Monster>();
        HashSet<Monster> surroundingMonsters = _map.FindMonstersInRadius(_actor.CurrentCoordsOriginCell, _actor.Stats.Size, attack.Reach);

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

        foreach (Monster enemy in _actor.VisibleTargets)
        {
            List<Coords> pathToEnemy = _map.FindPathForMonster(self:_actor, targetCell:null, targetMonster:enemy);

            if (pathToEnemy != null && pathToEnemy.Count > 0)
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
    public List<Coords> FindPathToClosestObstacle()
    {
        List<List<Coords>> pathsToAvailableObstacles = new List<List<Coords>>();

        foreach (Obstacle obstacle in _actor.CombatDependencies.Map.Obstacles)
        {
            List<Coords> pathToObstacle = _map.FindPathForMonster(self:_actor, targetCell:null, targetObstacle:obstacle);

            if (pathToObstacle != null && pathToObstacle.Count > 0)
                pathsToAvailableObstacles.Add(pathToObstacle);
        }

        if (pathsToAvailableObstacles.Count == 0)
            return null;

        List<Coords> pathToClosestObstacle = pathsToAvailableObstacles[0];
        for (int i = 1; i < pathsToAvailableObstacles.Count; i++)
        {
            List<Coords> currentPath = pathsToAvailableObstacles[i];

            if (currentPath.Count < pathToClosestObstacle.Count)
                pathToClosestObstacle = currentPath;
        }

        return pathToClosestObstacle;
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
        OnEnemyEndTurn?.Invoke();
    }
    protected void SwitchToCurrentAnimationClip(Monster animatedMonster, float currentClipLength)
    {
        if (animatedMonster == _actor)
        {
            _currentAnimationStartedPlaying = true;
            _currentAnimationLength = currentClipLength;
        }
    }
    public void SkipTurn()
    {
        OnEnemyEndTurn?.Invoke();
    }
}
