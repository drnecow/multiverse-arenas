using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public abstract class CombatAction : ScriptableObject
{
    [SerializeField] protected string _name = "Unspecified action";
    [SerializeField] protected CombatActionName _identifier;
    

    public virtual void DoAction(Monster actor, Monster target = null)
    {

    }
}
