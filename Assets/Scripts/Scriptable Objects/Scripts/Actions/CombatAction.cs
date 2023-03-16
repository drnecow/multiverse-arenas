using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public abstract class CombatAction : ScriptableObject
{
    [SerializeField] protected string _name = "Unspecified action";
    [SerializeField] protected CombatActionName _identifier;
    
    protected GridMap _map;

    public void SetMap(GridMap map)
    {
        _map = map;
    }

    public virtual void DoAction(Monster actor, Monster target = null)
    {

    }
}
