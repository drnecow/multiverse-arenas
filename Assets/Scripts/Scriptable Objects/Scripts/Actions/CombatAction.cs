using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public abstract class CombatAction : ScriptableObject
{
    [SerializeField] protected string _name = "Unspecified action";
    [field: SerializeField] public CombatActionType Identifier { get; protected set; }


    public virtual void DoAction(Monster actor)
    {

    }
    public virtual void DoAction(Monster actor, Monster target)
    {

    }
    public virtual void DoAction(Monster actor, List<Coords> path)
    {

    }
}
