using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.Dice;

public abstract class Attack : ScriptableObject
{
    [SerializeField] protected string _name = "Unspecified attack";
    [SerializeField] protected AttackName _identifier;
    [SerializeField] protected List<Die> _damageDice;
    [SerializeField] protected List<int> _numberOfDamageDice;
    [SerializeField] protected List<DamageType> _damageTypes;
    [SerializeField] protected Ability _usedAbility;
    [SerializeField] protected bool _isAbilityModifierAdded;
}
