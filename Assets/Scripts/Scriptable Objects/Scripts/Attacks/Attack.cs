using System;
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
    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public AttackType Identifier { get; protected set; }
    [field: SerializeField] public Ability UsedAbility { get; protected set; }
    [field: SerializeField] public AttackDamageInfo InitialDamageInfo { get; protected set; }
    [field: SerializeField] public bool IsAbilityModifierAdded { get; protected set; }
    [field: SerializeField] public List<AttackDamageInfo> AdditionalDamageInfo { get; protected set; }

    public event Action<ActionAnimationStartArguments> OnAttackAnimationStartedPlaying;
    public event Action OnAttackAnimationStoppedPlaying;


    protected CombatDependencies _combatDependencies;


    protected void MakeAttack(Monster actor, Monster target, RollMode rollMode)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        _combatDependencies.MonsterActionTracker.AddAttack(this);

        if (actor.ActiveConditions.Contains(Condition.Hiding))
        {
            actor.StealthRoll = -1000;
            actor.RemoveActiveCondition(Condition.Hiding);
            actor.MonsterAnimator.SetMonsterNormalMaterial();
            _combatDependencies.CombatManager.HandleMonsterBreakingStealth(actor);
            _combatDependencies.EventsLogger.LogLocalInfo(actor, "Stealth broken");
        }

        Debug.Log($"{actor.Name} is making {Name}");
        _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"{actor.Name} attacks {target.Name} with {Name}.");

        //Log attack name
        _combatDependencies.EventsLogger.LogLocalInfo(actor, Name);

        actor.MainActionAvailable = false;
        actor.AttackMainActionAvailable = false;
        actor.RemainingTotalAttacks--;
        actor.RemainingAttacks[this]--;

        //Play attack animation
        float animationClipLength = PlayAttackAnimation(actor, target);
        ActionAnimationStartArguments animationStartArguments = new ActionAnimationStartArguments(InvokeOnAttackAnimationStoppedPlaying, actor, animationClipLength);
        OnAttackAnimationStartedPlaying.Invoke(animationStartArguments);

        int toHitRoll = Dice.RollD20(1, rollMode);

        if (toHitRoll == 1)
        {
            _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"1: Critical Miss");
            target.MonsterAnimator.AnimateAvoidingDamage(actor);
        }
        else if (toHitRoll == 20)
        {
            _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"20: Critical Hit!");
            target.MonsterAnimator.AnimateTakingDamage();
            RollAndLogAttackDamage(actor, target, true);
        }
        else
        {
            int attackAbilityModifier = actor.Stats.Abilities.GetAbilityModifier(UsedAbility);
            int toHitNumber = toHitRoll + attackAbilityModifier + actor.Stats.ProficiencyBonus;

            if (toHitNumber >= target.Stats.ArmorClass)
            {
                _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"{toHitRoll}+{attackAbilityModifier}+{actor.Stats.ProficiencyBonus} >= {target.Stats.ArmorClass}: Hit");
                target.MonsterAnimator.AnimateTakingDamage();
                RollAndLogAttackDamage(actor, target, false);
            }
            else
            {
                _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"{toHitRoll}+{attackAbilityModifier}+{actor.Stats.ProficiencyBonus} < {target.Stats.ArmorClass}: Miss");
                target.MonsterAnimator.AnimateAvoidingDamage(actor);
            }
        }
    }

    private float PlayAttackAnimation(Monster actor, Monster target)
    {
        Coords actorOriginCell = actor.CurrentCoordsOriginCell;
        Coords targetOriginCell = target.CurrentCoordsOriginCell;

        Coords targetOffsetFromActor = new Coords(targetOriginCell.x - actorOriginCell.x, targetOriginCell.y - actorOriginCell.y);

        int xOffset = targetOffsetFromActor.x;
        int yOffset = -targetOffsetFromActor.y;

        SpriteRenderer actorSpriteRenderer = actor.gameObject.GetComponent<SpriteRenderer>();
        if (xOffset > 0)
            actorSpriteRenderer.flipX = true;
        else
            actorSpriteRenderer.flipX = false;

        if (xOffset != 0)
            xOffset /= Mathf.Abs(xOffset);
        if (yOffset != 0)
            yOffset /= Mathf.Abs(yOffset);

        Debug.Log($"Target offset from actor: ({xOffset}, {yOffset})");

        string clipName = $"{Name}Attack";
        float clipLength = FindAnimationClipLength(actor.Animator.runtimeAnimatorController, clipName);

        actor.Animator.SetTrigger(clipName);
        actor.Animator.SetFloat($"{Name}AttackTargetXOffset", xOffset);
        actor.Animator.SetFloat($"{Name}AttackTargetYOffset", yOffset);
     
        return clipLength;
    }
    public static int GetDamageDiceBonusForSize(Size size)
    {
        return size switch
        {
            Size.Tiny => 0,
            Size.Small => 0,
            Size.Medium => 0,
            Size.Large => 1,
            Size.Huge => 2,
            Size.Gargantuan => 3,
            _ => 0
        };
    }
    private void RollAndLogAttackDamage(Monster actor, Monster target, bool isCrit)
    {
        int attackAbilityModifier = actor.Stats.Abilities.GetAbilityModifier(UsedAbility);
        int damageDiceBonusForSize = GetDamageDiceBonusForSize(actor.Stats.Size);

        int initialDamage = Dice.RollDice(InitialDamageInfo.DamageDie, (InitialDamageInfo.NumberOfDamageDice + damageDiceBonusForSize) * (isCrit ? 2 : 1)) + (IsAbilityModifierAdded ? attackAbilityModifier : 0);
        int damageToTarget = target.TakeDamage(initialDamage, InitialDamageInfo.DamageType);

        //If it's crit, log critical damage; else, log normal damage
        _combatDependencies.EventsLogger.LogDamageNumber(target, damageToTarget, isCrit);
        _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"{target.Name} takes {damageToTarget} points of {InitialDamageInfo.DamageType} damage.");

        if (AdditionalDamageInfo.Count > 1)
        {
            foreach (AttackDamageInfo damageInfo in AdditionalDamageInfo)
            {
                int damage = Dice.RollDice(damageInfo.DamageDie, damageInfo.NumberOfDamageDice * (isCrit ? 2 : 1));
                damageToTarget = target.TakeDamage(damage, damageInfo.DamageType);
                //If it's crit, log critical damage; else, log normal damage
                _combatDependencies.EventsLogger.LogDamageNumber(target, damageToTarget, isCrit);
                _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"{target.Name} takes {damageToTarget} points of {damageInfo.DamageType} damage.");
            }
        }

        if (target.CurrentHP == 0)
            CombatDependencies.Instance.EventsLogger.Chat.LogEvent(target, $"{target.Name} dies.");
    }
    private float FindAnimationClipLength(RuntimeAnimatorController actorAnimatorController, string clipName)
    {
        foreach (AnimationClip clip in actorAnimatorController.animationClips)
            if (clip.name == clipName)
                return clip.length;

        return 0;
    }

    public void InvokeOnAttackAnimationStoppedPlaying()
    {
        OnAttackAnimationStoppedPlaying?.Invoke();
    }
}
