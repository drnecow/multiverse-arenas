using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public class ActionAnimationStartArguments
{
    public Action InvokeAnimationEndEvent { get; private set; }
    public Monster Actor { get; private set; }
    public float AnimationDurationInSeconds { get; private set; }

    public ActionAnimationStartArguments(Action invokeAnimationEndEvent, Monster actor, float animationDurationInSeconds)
    {
        InvokeAnimationEndEvent = invokeAnimationEndEvent;
        Actor = actor;
        AnimationDurationInSeconds = animationDurationInSeconds;
    }
}

public abstract class CombatAction : ScriptableObject
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public MonsterActionType Identifier { get; protected set; }

    public event Action<ActionAnimationStartArguments> OnActionAnimationStartedPlaying;
    public event Action OnActionAnimationStoppedPlaying;


    protected CombatDependencies _combatDependencies;


    public virtual void DoAction(Monster actor, CombatActionType consumedAction)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        Debug.Log($"MonsterActionTracker: {_combatDependencies.MonsterActionTracker}");
        _combatDependencies.MonsterActionTracker.AddCombatAction(this);
        _combatDependencies.EventsLogger.LogLocalInfo(actor, Name);

        if (consumedAction == CombatActionType.MainAction)
            actor.MainActionAvailable = false;
        else if (consumedAction == CombatActionType.BonusAction)
            actor.BonusActionAvailable = false;

        if (consumedAction != CombatActionType.FreeAction)
        {
            if (actor.ActiveConditions.Contains(Condition.Hiding)) {
                actor.StealthRoll = -1000;
                actor.RemoveActiveCondition(Condition.Hiding);
                actor.MonsterAnimator.SetMonsterNormalMaterial();
                _combatDependencies.CombatManager.HandleMonsterBreakingStealth(actor);
                _combatDependencies.EventsLogger.LogLocalInfo(actor, "Stealth broken", LogColor.Miss);
            }
        }

        if (Identifier != MonsterActionType.Move)
            _combatDependencies.EventsLogger.Chat.LogEvent(actor, $"{actor.Name} takes {Name} action.");
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

    protected void InvokeOnActionAnimationStartedPlaying(Monster actor, float animationDuration)
    {
        ActionAnimationStartArguments actionArguments = new ActionAnimationStartArguments(InvokeOnActionAnimationStoppedPlaying, actor, animationDuration);
        OnActionAnimationStartedPlaying?.Invoke(actionArguments);
    }
    public void InvokeOnActionAnimationStoppedPlaying()
    {
        OnActionAnimationStoppedPlaying?.Invoke();
    }
}
