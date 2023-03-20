using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Dice;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Scriptable Objects/Attack/Ranged Attack")]
public class RangedAttack : Attack
{
    [SerializeField] private int _normalRange;
    [SerializeField] private int _disadvantageRange;

    public void MakeRangedAttack(Monster actor, Monster target, int feetToTarget)
    {
        // Resolve advantage and disadvantage applied to ranged attack
        RollMode rollMode = actor.ResolveAdvantageAndDisadvantageToRangedAttack(target, feetToTarget);

        // Make the attack itself
        MakeAttack(actor, target, rollMode);
    }
}
