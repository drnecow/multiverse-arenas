using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;
using Project.Utils;

[CreateAssetMenu(fileName = "Move/Dash", menuName = "Scriptable Objects/Common Action/Move")]
public class Move : CombatAction
{
    [SerializeField] private float _speedPerCellInSeconds;

    // TODO: add opportunity attacks that trigger if actor is not disengaging

    public override void DoAction(Monster actor, List<Coords> path)
    {
        base.DoAction(actor);

        actor.MonsterAnimator.OnActionAnimationFinished -= HandleAnimationFinish;
        actor.MonsterAnimator.OnActionAnimationFinished += HandleAnimationFinish;

        SpriteRenderer actorSpriteRenderer = actor.gameObject.GetComponent<SpriteRenderer>();

        Coords initialCell = actor.CurrentCoordsOriginCell;
        Coords endPathCell = path[path.Count - 1];

        if (endPathCell.x < initialCell.x)
            actorSpriteRenderer.flipX = false;
        else if (endPathCell.x > initialCell.x)
            actorSpriteRenderer.flipX = true;


        Vector3 initialPosition = _combatDependencies.Map.XYToWorldPosition(initialCell);
        List<Vector3> pathPositions = path.ConvertAll((pathCell) => _combatDependencies.Map.XYToWorldPosition(pathCell));
        pathPositions.Insert(0, initialPosition);

        float animationDuration = path.Count * _speedPerCellInSeconds;
        OnActionAnimationStartedPlayingInvoke(actor, animationDuration + ConstantValues.ANIMATIONS_SWITCH_SPEED);

        _combatDependencies.Map.FreeCurrentCoordsOfMonster(actor);

        actor.Animator.SetTrigger(Name);
        actor.MonsterAnimator.MoveMonster(pathPositions, _speedPerCellInSeconds, MonsterActionType.Move);

        _combatDependencies.Map.PlaceMonsterOnCoords(actor, endPathCell);


        string pathString = "";

        foreach (Coords pathCoord in path)
            pathString += $"({pathCoord.x}, {pathCoord.y}) ";

        Debug.Log($"{actor.Name} is Moving along path {pathString}");
    }
    public override void DoAction(Monster actor, List<List<Coords>> path)
    {
        base.DoAction(actor);

        Debug.Log("Path: ");
        foreach(List<Coords> pathLine in path)
        {
            string lineString = "";

            foreach (Coords coord in pathLine)
                lineString += coord.ToString() + " ";

            Debug.Log(lineString);
        }

        actor.MonsterAnimator.OnActionAnimationFinished -= HandleAnimationFinish;
        actor.MonsterAnimator.OnActionAnimationFinished += HandleAnimationFinish;

        SpriteRenderer actorSpriteRenderer = actor.gameObject.GetComponent<SpriteRenderer>();

        int squareSide = GridMap.GetSquareSideForEntitySize(actor.Stats.Size);
        float cellOffset = ConstantValues.MAP_CELL_SIZE * squareSide / 2;

        Coords actorFirstCell = actor.CurrentCoordsOriginCell;
        Coords actorLastCell = new Coords(actorFirstCell.x + (squareSide - 1), actorFirstCell.y + (squareSide - 1));
        Vector3 actorFirstCellPosition = _combatDependencies.Map.XYToWorldPosition(actorFirstCell);
        Vector3 initialPosition;

        if ((actorLastCell.x - actorFirstCell.x) > 0)
            initialPosition = new Vector3(actorFirstCellPosition.x + cellOffset, actorFirstCellPosition.y);
        else
            initialPosition = new Vector3(actorFirstCellPosition.x, actorFirstCellPosition.y + cellOffset);

        Coords pathEndFirstCell = path[path.Count - 1][0];
        Coords pathEndLastCell = new Coords(pathEndFirstCell.x + (squareSide - 1), pathEndFirstCell.y + (squareSide - 1));
        Vector3 pathEndFirstCellPosition = _combatDependencies.Map.XYToWorldPosition(pathEndFirstCell);
        Vector3 pathEndPosition;

        if ((pathEndFirstCell.x - pathEndLastCell.x) > 0)
            pathEndPosition = new Vector3(pathEndFirstCellPosition.x + cellOffset, pathEndFirstCellPosition.y);
        else
            pathEndPosition = new Vector3(pathEndFirstCellPosition.x, pathEndFirstCellPosition.y + cellOffset);

        if (pathEndPosition.x < initialPosition.x)
            actorSpriteRenderer.flipX = false;
        else if (pathEndPosition.x > initialPosition.x)
            actorSpriteRenderer.flipX = true;

        List<Vector3> pathPositions = new List<Vector3>();

        foreach (List<Coords> pathLine in path)
        {
            Coords firstCell = pathLine[0];
            Coords lastCell = pathLine[squareSide - 1];
            Vector3 firstCellPosition = _combatDependencies.Map.XYToWorldPosition(firstCell);
            Vector3 monsterPosition;

            if ((lastCell.x - firstCell.x) > 0)
                monsterPosition = new Vector3(firstCellPosition.x + cellOffset, firstCellPosition.y);
            else
                monsterPosition = new Vector3(firstCellPosition.x, firstCellPosition.y + cellOffset);

            pathPositions.Add(monsterPosition);
        }

        pathPositions.Insert(0, initialPosition);

        float animationDuration = path.Count * _speedPerCellInSeconds;
        OnActionAnimationStartedPlayingInvoke(actor, animationDuration + ConstantValues.ANIMATIONS_SWITCH_SPEED);

        _combatDependencies.Map.FreeCurrentCoordsOfMonster(actor);

        actor.Animator.SetTrigger(Name);
        actor.MonsterAnimator.MoveMonster(pathPositions, _speedPerCellInSeconds, MonsterActionType.Move);

        _combatDependencies.Map.PlaceMonsterOnCoords(actor, pathEndFirstCell);

        string pathString = "";

        foreach (Vector3 position in pathPositions)
            pathString += $"({position.x}, {position.y}) ";

        Debug.Log($"{actor.Name} is Moving along path {pathString}");
    }

    private void HandleAnimationFinish(Monster actor, MonsterActionType actionType)
    {
        if (actionType == MonsterActionType.Move)
            actor.Animator.SetTrigger("StoppedMovement");
    }
}
