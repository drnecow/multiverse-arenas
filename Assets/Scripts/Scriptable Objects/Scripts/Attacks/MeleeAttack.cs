using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/Attack/Melee Attack")]
public class MeleeAttack : Attack
{
    [SerializeField] private int _reach;

    public void MakeMeleeAttack(Monster actor, Monster target)
    {

    }
}
