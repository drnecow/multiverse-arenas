using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Dice;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/Attack/Melee Attack")]
public class MeleeAttack : Attack
{
    [field: SerializeField] public int Reach { get; private set; }


    public void MakeMeleeAttack(Monster actor, Monster target)
    {
        // Resolve advantage and disadvantage applied to melee attack
        RollMode rollMode = actor.ResolveAdvantageAndDisadvantageToMeleeAttack(target);

        // Make the attack itself
        MakeAttack(actor, target, rollMode);
    }
}
