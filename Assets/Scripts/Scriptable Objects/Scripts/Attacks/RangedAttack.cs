using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Scriptable Objects/Attack/Ranged Attack")]
public class RangedAttack : Attack
{
    [SerializeField] private int _normalRange;
    [SerializeField] private int _disadvantageRange;

    public void MakeRangedAttack(Monster actor, Monster target, int feetToTarget)
    {

    }
}
