using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "Seek", menuName = "Scriptable Objects/Common Action/Seek")]
public class Seek : CombatAction
{
    private void Awake()
    {
        Name = "Seek";
        Identifier = MonsterActionType.Seek;
    }

    public override void DoAction(Monster actor)
    {
        base.DoAction(actor);
        

        Debug.Log($"{actor.Name} is Seeking");
    }
}
