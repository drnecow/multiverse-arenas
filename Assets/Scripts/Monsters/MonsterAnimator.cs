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
    public void MoveMonster(List<Vector3> pathPositions, float moveSpeedPerCellInSeconds, MonsterActionType movementType)
    {
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
        int attackerSquareSize = GridMap.GetSquareSideForEntitySize(attacker.Stats.Size);

        int xOffset = targetOffsetFromAttacker.x;
        int yOffset = targetOffsetFromAttacker.y;

        SpriteRenderer targetSpriteRenderer = _actor.gameObject.GetComponent<SpriteRenderer>();
        string clipName = "";

        // Conditions for front/back attack
        if (yOffset >= 0 && yOffset < attackerSquareSize)
        {
            if (xOffset < 0)
                targetSpriteRenderer.flipX = true;

            clipName = "DodgeBack";
        }
        // Conditions for up/down attack
        else if (xOffset >= 0 && xOffset < attackerSquareSize)
        {
            if (yOffset > 0)
                clipName = "DodgeDown";
            else
                clipName = $"DodgeUp";
        }
        // Conditions for diagonal up/diagonal down attack
        else
        {
            if (xOffset < 0)
                targetSpriteRenderer.flipX = true;

            if (yOffset > 0)
                clipName = $"DodgeDiagonalDown";
            else
                clipName = $"DodgeDiagonalUp";
        }
        Debug.Log("Avoiding damage");
        Debug.Log("Clip name: " + clipName);
        _actor.Animator.SetTrigger(clipName);
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
