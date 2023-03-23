using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.Dice;

[System.Serializable]
public struct AttackDamageInfo
{
    [field: SerializeField] public Die DamageDie { get; private set; }
    [field: SerializeField] public int NumberOfDamageDice { get; private set; }
    [field: SerializeField] public DamageType DamageType { get; private set; }
}

public abstract class Attack : ScriptableObject
{
    [SerializeField] protected string _name = "Unspecified attack";
    [field: SerializeField] public AttackType Identifier { get; protected set; }
    [SerializeField] protected Ability _usedAbility;
    [SerializeField] protected AttackDamageInfo _initialDamageInfo;
    [SerializeField] protected bool _isAbilityModifierAdded;
    [SerializeField] protected List<AttackDamageInfo> _additionalDamageInfo;

    protected void MakeAttack(Monster actor, Monster target, RollMode rollMode)
    {
        Debug.Log($"{actor.Name} is making {_name}");

        // TODO: log attack name

        int toHitRoll = Dice.RollD20(1, rollMode);
        // TODO: play attack animation

        if (toHitRoll == 1)
        {
            // TODO: log critical miss
            // TODO: play target dodging animation
        }
        else if (toHitRoll == 20)
        {
            // TODO: log critical hit

            RollAndLogAttackDamage(actor, target, true);
        }
        else
        {
            int attackAbilityModifier = actor.Stats.Abilities.GetAbilityModifier(_usedAbility);
            int toHitNumber = toHitRoll + attackAbilityModifier + actor.Stats.ProficiencyBonus;

            if (toHitNumber >= target.Stats.ArmorClass)
            {
                // TODO: log hit

                RollAndLogAttackDamage(actor, target, false);
            }
            else
            {
                // TODO: log miss
                // TODO: play target dodging animation
            }
        }
    }

    private void RollAndLogAttackDamage(Monster actor, Monster target, bool isCrit)
    {
        int attackAbilityModifier = actor.Stats.Abilities.GetAbilityModifier(_usedAbility);

        int damageDiceBonusForSize = actor.Stats.Size switch
        {
            Size.Tiny => 0,
            Size.Small => 0,
            Size.Medium => 0,
            Size.Large => 1,
            Size.Huge => 2,
            Size.Gargantuan => 3,
            _ => 0
        };

        int initialDamage = Dice.RollDice(_initialDamageInfo.DamageDie, (_initialDamageInfo.NumberOfDamageDice + damageDiceBonusForSize) * (isCrit ? 2 : 1)) + (_isAbilityModifierAdded ? attackAbilityModifier : 0);
        target.TakeDamage(initialDamage, _initialDamageInfo.DamageType);
        // TODO: if it's crit, log critical damage; else, log normal damage

        if (_additionalDamageInfo.Count > 1)
        {
            foreach (AttackDamageInfo damageInfo in _additionalDamageInfo)
            {
                int damage = Dice.RollDice(damageInfo.DamageDie, damageInfo.NumberOfDamageDice * (isCrit ? 2 : 1));
                target.TakeDamage(damage, damageInfo.DamageType);
                // TODO: if it's crit, log critical damage; else, log normal damage
            }
        }
    }
}
