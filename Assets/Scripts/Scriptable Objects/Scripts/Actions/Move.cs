using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Move/Dash", menuName = "Scriptable Objects/Common Action/Move")]
public class Move : CombatAction
{
    [SerializeField] private float _speedPerCellInSeconds;

    // TODO: add opportunity attacks that trigger if actor is not disengaging

    public override void DoAction(Monster actor, List<Coords> path, CombatActionType consumedAction)
    {
        base.DoAction(actor, consumedAction);

        actor.MonsterAnimator.OnActionAnimationFinished -= HandleAnimationFinish;
        actor.MonsterAnimator.OnActionAnimationFinished += HandleAnimationFinish;

        SpriteRenderer actorSpriteRenderer = actor.gameObject.GetComponent<SpriteRenderer>();

        Coords initialCell = actor.CurrentCoordsOriginCell;
        Coords endPathCell = path[path.Count - 1];

        if (endPathCell.x < initialCell.x)
            actorSpriteRenderer.flipX = false;
        else if (endPathCell.x > initialCell.x)
            actorSpriteRenderer.flipX = true;

        path.Insert(0, initialCell);

        float animationDuration = path.Count * _speedPerCellInSeconds;
        OnActionAnimationStartedPlayingInvoke(actor, animationDuration + ConstantValues.ANIMATIONS_SWITCH_SPEED);

        _combatDependencies.Map.FreeCurrentCoordsOfMonster(actor);

        actor.Animator.SetTrigger(Name);
        actor.MonsterAnimator.MoveMonster(path, _speedPerCellInSeconds, MonsterActionType.Move);

        _combatDependencies.Map.PlaceMonsterOnCoords(actor, endPathCell);

        if (Identifier == MonsterActionType.Move)
            actor.RemainingSpeed.Walk -= path.Count * 5;


        string pathString = "";

        foreach (Coords pathCoord in path)
            pathString += $"({pathCoord.x}, {pathCoord.y}) ";

        Debug.Log($"{actor.Name} is Moving along path {pathString}");
    }

    private void HandleAnimationFinish(Monster actor, MonsterActionType actionType)
    {
        if (actionType == MonsterActionType.Move)
            actor.Animator.SetTrigger("StoppedMovement");
    }

    public override bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType usedAction)
    {
        switch (Identifier)
        {
            case MonsterActionType.Move:
                return actor.RemainingSpeed.Walk >= 5;

            case MonsterActionType.Dash:
                switch (usedAction)
                {
                    case CombatActionType.FreeAction:
                        return true;
                    case CombatActionType.BonusAction:
                        return actor.BonusActionAvailable;
                    case CombatActionType.MainAction:
                        return actor.MainActionAvailable;
                    default:
                        return false;
                }

            default:
                return false;
        }
    }
}
