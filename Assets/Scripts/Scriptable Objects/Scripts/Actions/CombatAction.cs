using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public abstract class CombatAction : ScriptableObject
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public MonsterActionType Identifier { get; protected set; }

    public event Action<Monster, float> OnActionAnimationStartedPlaying;


    protected CombatDependencies _combatDependencies;


    public virtual void DoAction(Monster actor, CombatActionType consumedAction)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, Name);

        if (consumedAction == CombatActionType.MainAction)
            actor.MainActionAvailable = false;
        else if (consumedAction == CombatActionType.BonusAction)
            actor.BonusActionAvailable = false;

        if (consumedAction != CombatActionType.FreeAction)
        {
            if (actor.StealthRoll > -1000)
            {
                actor.StealthRoll = -1000;
                _combatDependencies.CombatManager.HandleMonsterBreakingStealth(actor);
                _combatDependencies.EventsLogger.LogLocalInfo(actor, "Stealth broken");
            }
        }
    }
    public virtual void DoAction(Monster actor, Monster target, CombatActionType consumedAction)
    {
    }
    public virtual void DoAction(Monster actor, List<Coords> path, CombatActionType consumedAction)
    {
    }
    public virtual void DoAction(Monster actor, List<List<Coords>> path, CombatActionType consumedAction)
    {
    }

    public virtual bool DoPlayerButtonAction()
    {
        return false;
    }

    protected float FindAnimationClipLength(RuntimeAnimatorController actorAnimatorController)
    {
        foreach (AnimationClip clip in actorAnimatorController.animationClips)
            if (clip.name == Name)
                return clip.length;

        return 0;
    }

    public virtual bool CheckPlayerButtonInteractabilityCondition(Monster actor, CombatActionType consumedAction)
    {
        return consumedAction switch
        {
            CombatActionType.FreeAction => true,
            CombatActionType.BonusAction => actor.BonusActionAvailable,
            CombatActionType.MainAction => actor.MainActionAvailable,
            _ => false,
        };
    }

    protected void OnActionAnimationStartedPlayingInvoke(Monster actor, float animationDuration)
    {
        OnActionAnimationStartedPlaying?.Invoke(actor, animationDuration);
    }
}
