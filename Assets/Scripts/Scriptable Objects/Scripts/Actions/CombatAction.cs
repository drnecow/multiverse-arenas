using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public abstract class CombatAction : ScriptableObject
{
    [SerializeField] protected string _name = "Unspecified action";
    [field: SerializeField] public CombatActionType Identifier { get; protected set; }

    public event Action<float> OnActionAnimationStartedPlaying;


    protected CombatDependencies _combatDependencies;


    public virtual void DoAction(Monster actor)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, _name);
    }
    public virtual void DoAction(Monster actor, Monster target)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, _name);
    }
    public virtual void DoAction(Monster actor, List<Coords> path)
    {
        if (_combatDependencies == null)
            _combatDependencies = CombatDependencies.Instance;

        CombatDependencies.Instance.EventsLogger.LogLocalInfo(actor, _name);
    }

    protected float FindAnimationClipLength(RuntimeAnimatorController actorAnimatorController)
    {
        foreach (AnimationClip clip in actorAnimatorController.animationClips)
            if (clip.name == _name)
                return clip.length;

        return 0;
    }
}
