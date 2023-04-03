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


    public virtual void DoAction(Monster actor)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, Name);
    }
    public virtual void DoAction(Monster actor, Monster target)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, Name);
    }
    public virtual void DoAction(Monster actor, List<Coords> path)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, Name);
    }
    public virtual void DoAction(Monster actor, List<List<Coords>> path)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, Name);
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

    protected void OnActionAnimationStartedPlayingInvoke(Monster actor, float animationDuration)
    {
        OnActionAnimationStartedPlaying?.Invoke(actor, animationDuration);
    }
}
