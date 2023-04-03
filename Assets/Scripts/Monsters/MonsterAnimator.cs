using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public class MonsterAnimator : MonoBehaviour
{
    private Monster _actor;
    private Animator _animator;

    [SerializeField] Sprite _deathSprite;

    [SerializeField] List<string> _actorAttacksStringIdentifiers;

    public event Action<Monster, MonsterActionType> OnActionAnimationFinished;


    private void Awake()
    {
        _actor = gameObject.GetComponent<Monster>();
        _animator = _actor.Animator;
    }

    public void KillMonster()
    {
        _animator.SetTrigger("Die");
        //_actor.gameObject.GetComponent<SpriteRenderer>().sprite = _deathSprite;
    }
    public void MoveMonster(List<Vector3> pathPositions, float moveSpeedPerCellInSeconds, MonsterActionType movementType)
    {
        StartCoroutine(AnimateMonsterMovement(pathPositions, moveSpeedPerCellInSeconds, movementType));
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
}
