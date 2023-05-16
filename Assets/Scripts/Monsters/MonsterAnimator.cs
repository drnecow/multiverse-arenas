using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public class MonsterAnimator : MonoBehaviour
{
    private Monster _actor;
    private Animator _animator;
    [SerializeField] private VisualAssets _visualAssets;

    [field: SerializeField] public Sprite IdleSprite { get; private set; }

    public event Action<Monster, MonsterActionType> OnActionAnimationFinished;


    private void Awake()
    {
        _actor = gameObject.GetComponent<Monster>();
        _animator = _actor.Animator;
    }

    public void KillMonster()
    {
        _animator.SetTrigger("Die");
        SetMonsterNormalMaterial();
        _actor.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Dead monsters";
    }
    public void MoveMonster(List<Coords> pathCells, float moveSpeedPerCellInSeconds, MonsterActionType movementType)
    {
        int actorSquareSide = GridMap.GetSquareSideForEntitySize(_actor.Stats.Size);
        List<Vector3> pathPositions = new List<Vector3>();

        string pathCellsString = "";
        foreach (Coords cell in pathCells)
        {
            pathPositions.Add(GridMap.GetCenterOfSquare(cell, actorSquareSide));
            pathCellsString += $"{cell} ";
        }

        string pathPositionsString = "";
        foreach (Vector3 position in pathPositions)
            pathPositionsString += $"{position} ";

        //Debug.Log("Path: " + pathCellsString);
        //Debug.Log("Path positions: " + pathPositionsString);

        StartCoroutine(AnimateMonsterMovement(pathPositions, moveSpeedPerCellInSeconds, movementType));
    }
    public void AnimateTakingDamage()
    {
        _animator.SetTrigger("TakeDamage");
    }
    public void AnimateAvoidingDamage(Monster attacker)
    {
        Coords attackerOriginCell = attacker.CurrentCoordsOriginCell;
        Coords targetOriginCell = _actor.CurrentCoordsOriginCell;

        Coords targetOffsetFromAttacker = new Coords(targetOriginCell.x - attackerOriginCell.x, targetOriginCell.y - attackerOriginCell.y);

        int xOffset = targetOffsetFromAttacker.x;
        int yOffset = -targetOffsetFromAttacker.y;

        SpriteRenderer targetSpriteRenderer = _actor.gameObject.GetComponent<SpriteRenderer>();
        if (xOffset < 0)
            targetSpriteRenderer.flipX = true;
        else
            targetSpriteRenderer.flipX = false;

        if (xOffset != 0)
            xOffset /= Mathf.Abs(xOffset);
        if (yOffset != 0)
            yOffset /= Mathf.Abs(yOffset);

        _actor.Animator.SetTrigger("Dodge");
        _actor.Animator.SetFloat("DodgeXOffset", xOffset);
        _actor.Animator.SetFloat("DodgeYOffset", yOffset);
    }
    private IEnumerator AnimateMonsterMovement(List<Vector3> pathPositions, float moveSpeedPerCellInSeconds, MonsterActionType movementType)
    {
        float moveSpeedPerIteration = moveSpeedPerCellInSeconds / 5;
        SpriteRenderer actorSpriteRenderer = _actor.gameObject.GetComponent<SpriteRenderer>();

        for (int i = 0; i < pathPositions.Count - 1; i++)
        {
            Vector3 currentPosition = pathPositions[i];
            Vector3 nextPosition = pathPositions[i + 1];

            if (nextPosition.x < currentPosition.x)
                actorSpriteRenderer.flipX = false;
            else if (nextPosition.x > currentPosition.x)
                actorSpriteRenderer.flipX = true;

            for (int iterationStep = 1; iterationStep < 6; iterationStep++)
            {
                float newXPosition = Mathf.Lerp(currentPosition.x, nextPosition.x, 0.25f * iterationStep);
                float newYPosition = Mathf.Lerp(currentPosition.y, nextPosition.y, 0.25f * iterationStep);

                _actor.transform.position = new Vector3(newXPosition, newYPosition);
                yield return new WaitForSeconds(moveSpeedPerIteration);
            }
        }

        if (movementType == MonsterActionType.Move)
            OnActionAnimationFinished?.Invoke(_actor, MonsterActionType.Move);
        else if (movementType == MonsterActionType.Dash)
            OnActionAnimationFinished?.Invoke(_actor, MonsterActionType.Dash);
    }
    public void SetMonsterStealthMaterial()
    {
        _actor.gameObject.GetComponent<SpriteRenderer>().material = _visualAssets.StealthMaterial;
    }
    public void SetMonsterNormalMaterial()
    {
        _actor.gameObject.GetComponent<SpriteRenderer>().material = _visualAssets.NormalMaterial;
    }
}
