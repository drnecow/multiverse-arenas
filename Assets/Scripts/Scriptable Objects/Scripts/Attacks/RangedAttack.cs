using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Dice;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Scriptable Objects/Attack/Ranged Attack")]
public class RangedAttack : Attack
{
    [field: SerializeField] public int NormalRange { get; private set; }
    [field: SerializeField] public int DisadvantageRange { get; private set; }


    public void MakeRangedAttack(Monster actor, Monster target, int feetToTarget)
    {
        // Resolve advantage and disadvantage applied to ranged attack
        RollMode rollMode = actor.ResolveAdvantageAndDisadvantageToRangedAttack(target, feetToTarget);

        // Make the attack itself
        MakeAttack(actor, target, rollMode);
    }
}
