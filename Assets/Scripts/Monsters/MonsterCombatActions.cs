using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[System.Serializable]
public class MonsterCombatActions
{
    [SerializeField] private List<CombatAction> _freeActions;
    [SerializeField] private List<CombatAction> _mainActions;
    [SerializeField] private List<CombatAction> _bonusActions;

    [SerializeField] private MeleeAttackIntDictionary _meleeAttacks;
    [SerializeField] private RangedAttackIntDictionary _rangedAttacks;


    public CombatAction FindFreeActionOfType(CombatActionType actionType)
    {
        foreach (CombatAction freeAction in _freeActions)
            if (freeAction.Identifier == actionType)
                return freeAction;

        return null;
    }
    public CombatAction FindMainActionOfType(CombatActionType actionType)
    {
        foreach (CombatAction mainAction in _mainActions)
            if (mainAction.Identifier == actionType)
                return mainAction;

        return null;
    }
    public CombatAction FindBonusActionOfType(CombatActionType actionType)
    {
        foreach (CombatAction bonusAction in _bonusActions)
            if (bonusAction.Identifier == actionType)
                return bonusAction;

        return null;
    }

    public MeleeAttack FindMeleeAttackOfType(AttackType attackType)
    {
        foreach (MeleeAttack meleeAttack in _meleeAttacks.Keys)
            if (meleeAttack.Identifier == attackType)
                return meleeAttack;

        return null;
    }
    public RangedAttack FindRangedAttackOfType(AttackType attackType)
    {
        foreach (RangedAttack rangedAttack in _rangedAttacks.Keys)
            if (rangedAttack.Identifier == attackType)
                return rangedAttack;

        return null;
    }
}
