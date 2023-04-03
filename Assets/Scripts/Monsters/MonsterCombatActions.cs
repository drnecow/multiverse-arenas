using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[System.Serializable]
public class MonsterCombatActions
{
    [field: SerializeField] public List<CombatAction> FreeActions { get; private set; }
    [field: SerializeField] public List<CombatAction> MainActions { get; private set; }
    [field: SerializeField] public List<CombatAction> BonusActions { get; private set; }

    [field: SerializeField] public MeleeAttackIntDictionary MeleeAttacks { get; private set; }
    [field: SerializeField] public RangedAttackIntDictionary RangedAttacks { get; private set; }


    public CombatAction FindFreeActionOfType(MonsterActionType actionType)
    {
        foreach (CombatAction freeAction in FreeActions)
            if (freeAction.Identifier == actionType)
                return freeAction;

        return null;
    }
    public CombatAction FindMainActionOfType(MonsterActionType actionType)
    {
        foreach (CombatAction mainAction in MainActions)
            if (mainAction.Identifier == actionType)
                return mainAction;

        return null;
    }
    public CombatAction FindBonusActionOfType(MonsterActionType actionType)
    {
        foreach (CombatAction bonusAction in BonusActions)
            if (bonusAction.Identifier == actionType)
                return bonusAction;

        return null;
    }

    public MeleeAttack FindMeleeAttackOfType(AttackType attackType)
    {
        foreach (MeleeAttack meleeAttack in MeleeAttacks.Keys)
            if (meleeAttack.Identifier == attackType)
                return meleeAttack;

        return null;
    }
    public RangedAttack FindRangedAttackOfType(AttackType attackType)
    {
        foreach (RangedAttack rangedAttack in RangedAttacks.Keys)
            if (rangedAttack.Identifier == attackType)
                return rangedAttack;

        return null;
    }
}
